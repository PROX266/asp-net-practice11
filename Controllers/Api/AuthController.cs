using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using WebApplication1.DTOs.Auth;
using WebApplication1.Services;

namespace WebApplication1.Controllers.Api;

[ApiController]
[Route("api/auth")]
[IgnoreAntiforgeryToken]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ITokenService _tokenService;

    public AuthController(IUserService userService, ITokenService tokenService)
    {
        _userService = userService;
        _tokenService = tokenService;
    }

    // POST /api/auth/register
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userService.RegisterAsync(dto.Login, dto.Password, cancellationToken);
            var accessToken = _tokenService.CreateAccessToken(user);

            return Ok(new LoginResponseDto
            {
                AccessToken = accessToken,
                ExpiresInMinutes = _tokenService.ExpiresInMinutes,
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // POST /api/auth/login
    // Ограничен rate limiting-политикой "LoginPolicy" (см. Program.cs) —
    // не больше 5 попыток в минуту с одного адреса, дальше 429.
    [EnableRateLimiting("LoginPolicy")]
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto, CancellationToken cancellationToken)
    {
        var user = await _userService.ValidateCredentialsAsync(
            dto.Login, dto.Password, cancellationToken);
        if (user is null)
            return Unauthorized(new { error = "Неверный логин или пароль." });

        var accessToken = _tokenService.CreateAccessToken(user);

        return Ok(new LoginResponseDto
        {
            AccessToken = accessToken,
            ExpiresInMinutes = _tokenService.ExpiresInMinutes,
        });
    }

    // GET /api/auth/me — защищённый эндпоинт: любой авторизованный пользователь
    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        return Ok(new
        {
            id = User.FindFirstValue(ClaimTypes.NameIdentifier),
            login = User.Identity?.Name,
            role = User.FindFirstValue(ClaimTypes.Role),
        });
    }
}
