using WebApplication1.DTOs;

namespace WebApplication1.Services;

public interface ITokenService
{
    int ExpiresInMinutes { get; }

    string CreateAccessToken(UserDto user);
}
