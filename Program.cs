using System.Globalization;
using System.Text;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using WebApplication1.Repositories;
using WebApplication1.Security;
using WebApplication1.Services;
using WebApplication1.Swagger;

namespace WebApplication1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews(options =>
            {
                options.ModelBindingMessageProvider.SetAttemptedValueIsInvalidAccessor((value, field) => $"Значение '{value}' недопустимо для поля {field}");
                options.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor(field => "Поле обязательно для заполнения");

                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
            });

            builder.Services.AddDbContext<WebApplication1.Models.WebstoreContext>(option =>
                option.UseSqlServer(builder.Configuration.GetConnectionString("con")));

            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped<IRoleRepository, RoleRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<ICartRepository, CartRepository>();
            builder.Services.AddScoped<ICartItemRepository, CartItemRepository>();
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            builder.Services.AddScoped<IOrderItemRepository, OrderItemRepository>();
            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<IProductReservationRepository, ProductReservationRepository>();

            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<IRoleService, RoleService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<ICartService, CartService>();
            builder.Services.AddScoped<ICartItemService, CartItemService>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<IOrderItemService, OrderItemService>();
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<IProductReservationService, ProductReservationService>();
            builder.Services.AddScoped<ITokenService, TokenService>();

            builder.Services.AddAutoMapper(cfg => cfg.AddMaps(typeof(Program)));

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Введите: Bearer {ваш JWT-токен}",
                });
                
                options.OperationFilter<AuthorizeCheckOperationFilter>();
            });

            builder.Services.AddMemoryCache();
            builder.Services.AddScoped<CatalogCache>();

            builder.Services.Configure<FormOptions>(o => o.MultipartBodyLengthLimit = 3 * 1024 * 1024);

            builder.Services.AddDataProtection()
                .SetApplicationName("WebApplication1")
                .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(builder.Environment.ContentRootPath, "keys")));

            var jwtSection = builder.Configuration.GetSection("Jwt");
            
            builder.Services.AddAuthentication(options =>
                {
                    options.DefaultScheme = "CookieOrJwt";
                    options.DefaultChallengeScheme = "CookieOrJwt";
                })
                .AddPolicyScheme("CookieOrJwt", "Cookie или JWT", options =>
                {
                    options.ForwardDefaultSelector = ctx =>
                        ctx.Request.Path.StartsWithSegments("/api")
                            ? JwtBearerDefaults.AuthenticationScheme
                            : CookieAuthenticationDefaults.AuthenticationScheme;
                })
                .AddCookie(options =>
                {
                    options.LoginPath = "/Account/Login";
                    options.AccessDeniedPath = "/Account/AccessDenied";
                    options.Cookie.Name = "WebstoreAuth";
                    options.Cookie.HttpOnly = true;
                    options.Cookie.SameSite = SameSiteMode.Lax;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                    options.ExpireTimeSpan = TimeSpan.FromHours(1);
                    options.SlidingExpiration = true;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtSection["Issuer"],
                        ValidAudience = jwtSection["Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(jwtSection["Key"]!)),
                        ClockSkew = TimeSpan.Zero
                    };
                });

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy(Policies.CanManageCatalog, policy =>
                    policy.RequireRole(AppRoles.Admin, AppRoles.Manager));

                options.AddPolicy(Policies.AdminOnly, policy =>
                    policy.RequireRole(AppRoles.Admin));
                
                options.AddPolicy(Policies.OrderOwnerOrAdmin, policy =>
                    policy.Requirements.Add(new OrderOwnerRequirement()));
            });
            builder.Services.AddSingleton<IAuthorizationHandler, OrderOwnerHandler>();

            const string FrontendPolicy = "FrontendPolicy";
            var allowedOrigins = builder.Configuration
                .GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy(FrontendPolicy, policy =>
                {
                    policy
                        .WithOrigins(allowedOrigins)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .WithExposedHeaders("X-Total-Count", "Retry-After");
                });
            });

            builder.Services.AddRateLimiter(options =>
            {
                options.AddPolicy("LoginPolicy", httpContext =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                        _ => new FixedWindowRateLimiterOptions
                        {
                            Window = TimeSpan.FromMinutes(1),
                            PermitLimit = 5,
                            QueueLimit = 0
                        }));

                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                options.OnRejected = async (context, token) =>
                {
                    if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                        context.HttpContext.Response.Headers["Retry-After"] =
                            ((int)retryAfter.TotalSeconds).ToString();

                    context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    await context.HttpContext.Response.WriteAsJsonAsync(
                        new { error = "Слишком много попыток входа. Повторите позже." }, token);
                };
            });

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseWhen(
                ctx => !ctx.Request.Path.StartsWithSegments("/api"),
                branch => branch.UseStatusCodePagesWithReExecute("/Home/StatusCode", "?code={0}"));

            app.UseHttpsRedirection();

            app.Use(async (context, next) =>
            {
                var h = context.Response.Headers;
                h["X-Content-Type-Options"] = "nosniff";
                h["X-Frame-Options"] = "DENY";
                h["Referrer-Policy"] = "strict-origin-when-cross-origin";
                if (!context.Request.Path.StartsWithSegments("/swagger"))
                {
                    h["Content-Security-Policy"] =
                        "default-src 'self'; img-src 'self' data:; style-src 'self' 'unsafe-inline'; script-src 'self'; frame-ancestors 'none'";
                }
                await next();
            });

            app.UseStaticFiles();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            var ruCulture = new CultureInfo("ru-RU");
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(ruCulture),
                SupportedCultures = new List<CultureInfo> { ruCulture },
                SupportedUICultures = new List<CultureInfo> { ruCulture }
            });

            app.UseRouting();
            
            app.UseCors(FrontendPolicy);
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseRateLimiter();

            app.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            if (app.Environment.IsDevelopment())
            {
                using var scope = app.Services.CreateScope();
                DevDataSeeder.Seed(scope.ServiceProvider);
            }

            app.Run();
        }
    }
}
