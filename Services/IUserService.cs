using WebApplication1.DTOs;

namespace WebApplication1.Services;

public interface IUserService
{
    Task<(List<UserDto> Items, int TotalCount)> GetAllAsync(
        int page, int pageSize, CancellationToken ct);

    Task<UserDto?> GetByIdAsync(int id, CancellationToken ct);

    Task<UserDto> CreateAsync(CreateUserDto dto, CancellationToken ct);

    Task<UserDto?> UpdateAsync(int id, UpdateUserDto dto, CancellationToken ct);

    Task<UserDto?> PatchAsync(int id, PatchUserDto dto, CancellationToken ct);

    Task<bool> DeleteAsync(int id, CancellationToken ct);

    // Регистрация и проверка пароля для JWT-входа
    Task<UserDto> RegisterAsync(string login, string password, CancellationToken ct);

    Task<UserDto?> ValidateCredentialsAsync(string login, string password, CancellationToken ct);
}
