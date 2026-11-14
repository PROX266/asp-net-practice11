using AutoMapper;
using Microsoft.AspNetCore.Identity;
using WebApplication1.DTOs;
using WebApplication1.Models;
using WebApplication1.Repositories;

namespace WebApplication1.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _repository;
    private readonly IMapper _mapper;
    private readonly PasswordHasher<User> _hasher = new();

    public UserService(IUserRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<(List<UserDto> Items, int TotalCount)> GetAllAsync(
        int page, int pageSize, CancellationToken ct)
    {
        var users = await _repository.GetAllAsync(page, pageSize, ct);
        var total = await _repository.CountAsync(ct);
        return (_mapper.Map<List<UserDto>>(users), total);
    }

    public async Task<UserDto?> GetByIdAsync(int id, CancellationToken ct)
    {
        var user = await _repository.GetByIdAsync(id, ct);
        return user is null ? null : _mapper.Map<UserDto>(user);
    }

    public async Task<UserDto> CreateAsync(CreateUserDto dto, CancellationToken ct)
    {
        _ = await _repository.FindRoleAsync(dto.RoleId, ct)
            ?? throw new InvalidOperationException(
                $"Роль с Id {dto.RoleId} не найдена.");

        var user = _mapper.Map<User>(dto);
        user.UserName = dto.UserLogin;
        user.UserPassword = PasswordHelper.Hash(dto.Password);
        user.PasswordHash = _hasher.HashPassword(user, dto.Password);
        user.IsActive = true;

        await _repository.AddAsync(user);
        await _repository.SaveChangesAsync();

        var saved = await _repository.GetByIdAsync(user.Id, ct)
            ?? throw new InvalidOperationException(
                "Не удалось загрузить только что созданного пользователя.");
        return _mapper.Map<UserDto>(saved);
    }

    public async Task<UserDto?> UpdateAsync(
        int id, UpdateUserDto dto, CancellationToken ct)
    {
        var user = await _repository.GetByIdAsync(id, ct);
        if (user is null) return null;

        _ = await _repository.FindRoleAsync(dto.RoleId, ct)
            ?? throw new InvalidOperationException(
                $"Роль с Id {dto.RoleId} не найдена.");

        _mapper.Map(dto, user);

        _repository.Update(user);
        await _repository.SaveChangesAsync();

        var saved = await _repository.GetByIdAsync(id, ct)
            ?? throw new InvalidOperationException(
                "Не удалось загрузить обновлённого пользователя.");
        return _mapper.Map<UserDto>(saved);
    }

    public async Task<UserDto?> PatchAsync(
        int id, PatchUserDto dto, CancellationToken ct)
    {
        var user = await _repository.GetByIdAsync(id, ct);
        if (user is null) return null;

        if (dto.UserLogin is not null) user.UserLogin = dto.UserLogin;

        if (dto.RoleId is not null)
        {
            _ = await _repository.FindRoleAsync(dto.RoleId.Value, ct)
                ?? throw new InvalidOperationException(
                    $"Роль с Id {dto.RoleId} не найдена.");
            user.RoleId = dto.RoleId.Value;
        }

        _repository.Update(user);
        await _repository.SaveChangesAsync();

        var saved = await _repository.GetByIdAsync(id, ct)
            ?? throw new InvalidOperationException(
                "Не удалось загрузить обновлённого пользователя.");
        return _mapper.Map<UserDto>(saved);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct)
    {
        var user = await _repository.GetByIdAsync(id, ct);
        if (user is null) return false;

        _repository.Remove(user);
        await _repository.SaveChangesAsync();
        return true;
    }

    public async Task<UserDto> RegisterAsync(string login, string password, CancellationToken ct)
    {
        if (await _repository.LoginExistsAsync(login, ct))
            throw new InvalidOperationException($"Логин '{login}' уже занят.");

        var role = await _repository.FindRoleByNameAsync("User", ct)
            ?? throw new InvalidOperationException(
                "Роль 'User' не найдена.");

        var user = new User
        {
            UserLogin = login,
            UserName = login,
            RoleId = role.Id,
            IsActive = true,
            UserPassword = Array.Empty<byte>()
        };
        user.PasswordHash = _hasher.HashPassword(user, password);

        await _repository.AddAsync(user);
        await _repository.SaveChangesAsync();

        var saved = await _repository.GetByIdAsync(user.Id, ct)
            ?? throw new InvalidOperationException(
                "Не удалось загрузить только что созданного пользователя.");
        return _mapper.Map<UserDto>(saved);
    }

    public async Task<UserDto?> ValidateCredentialsAsync(
        string login, string password, CancellationToken ct)
    {
        var user = await _repository.GetByLoginAsync(login, ct);
        if (user is null || !user.IsActive) return null;

        if (!string.IsNullOrEmpty(user.PasswordHash))
        {
            var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, password);
            if (result == PasswordVerificationResult.Failed) return null;

            if (result == PasswordVerificationResult.SuccessRehashNeeded)
            {
                user.PasswordHash = _hasher.HashPassword(user, password);
                await _repository.SaveChangesAsync();
            }
        }
        else
        {
            if (!PasswordHelper.Verify(password, user.UserPassword)) return null;

            user.PasswordHash = _hasher.HashPassword(user, password);
            await _repository.SaveChangesAsync();
        }

        return _mapper.Map<UserDto>(user);
    }
}
