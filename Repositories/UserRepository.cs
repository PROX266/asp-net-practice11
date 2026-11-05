using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Repositories;

public class UserRepository : IUserRepository
{
    private readonly WebstoreContext _context;

    public UserRepository(WebstoreContext context)
    {
        _context = context;
    }

    public async Task<List<User>> GetAllAsync(int page, int pageSize, CancellationToken ct)
    {
        return await _context.Users
            .AsNoTracking()
            .OrderBy(u => u.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Include(u => u.Role)
            .ToListAsync(ct);
    }

    public async Task<int> CountAsync(CancellationToken ct) =>
        await _context.Users.CountAsync(ct);

    public async Task<User?> GetByIdAsync(int id, CancellationToken ct)
    {
        return await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == id, ct);
    }

    public async Task<Role?> FindRoleAsync(int roleId, CancellationToken ct) =>
        await _context.Roles.FirstOrDefaultAsync(r => r.Id == roleId, ct);

    public async Task<User?> FindUserAsync(int userId, CancellationToken ct) =>
        await _context.Users.FirstOrDefaultAsync(u => u.Id == userId, ct);

    public async Task<User?> GetByLoginAsync(string login, CancellationToken ct) =>
        await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.UserLogin == login, ct);

    public async Task<bool> LoginExistsAsync(string login, CancellationToken ct) =>
        await _context.Users.AnyAsync(u => u.UserLogin == login, ct);

    public async Task<Role?> FindRoleByNameAsync(string roleName, CancellationToken ct) =>
        await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == roleName, ct);

    public async Task AddAsync(User user) =>
        await _context.Users.AddAsync(user);

    public void Update(User user) => _context.Users.Update(user);

    public void Remove(User user) => _context.Users.Remove(user);

    public async Task SaveChangesAsync() =>
        await _context.SaveChangesAsync();
}
