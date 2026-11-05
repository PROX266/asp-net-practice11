using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WebApplication1.Swagger;

// Вешает замок (security requirement) только на методы, где реально стоит [Authorize]
// и нет [AllowAnonymous]. Открытые методы остаются без замка.
public class AuthorizeCheckOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var metadata = context.ApiDescription.ActionDescriptor.EndpointMetadata;

        var requiresAuth = metadata.OfType<IAuthorizeData>().Any();
        var allowsAnonymous = metadata.OfType<IAllowAnonymous>().Any();
        if (!requiresAuth || allowsAnonymous) return;

        operation.Responses.TryAdd("401", new OpenApiResponse { Description = "Не авторизован (нет или неверный токен)" });
        operation.Responses.TryAdd("403", new OpenApiResponse { Description = "Недостаточно прав" });

        operation.Security = new List<OpenApiSecurityRequirement>
        {
            new()
            {
                [new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                }] = Array.Empty<string>()
            }
        };
    }
}
