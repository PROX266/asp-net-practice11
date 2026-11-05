using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using WebApplication1.DTOs;

namespace WebApplication1.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _config;

    public TokenService(IConfiguration config)
    {
        _config = config;
    }

    public int ExpiresInMinutes =>
        (int)double.Parse(_config.GetSection("Jwt")["ExpiresMinutes"]!);

    public string CreateAccessToken(UserDto user)
    {
        var jwt = _config.GetSection("Jwt");

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserLogin),
            new(ClaimTypes.Role, user.Role.RoleName),
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwt["Key"]!));
        var creds = new SigningCredentials(
            key, SecurityAlgorithms.HmacSha256);

        var minutes = double.Parse(jwt["ExpiresMinutes"]!);

        var token = new JwtSecurityToken(
            issuer: jwt["Issuer"],
            audience: jwt["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(minutes),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
