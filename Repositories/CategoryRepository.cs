using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly WebstoreContext _context;

    public CategoryRepository(WebstoreContext context)
    {
        _context = context;
    }

    public async Task<List<Category>> GetAllAsync(int page, int pageSize, CancellationToken ct)
    {
        return await _context.Categories
            .AsNoTracking()
            .OrderBy(c => c.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Include(c => c.Products)
            .ToListAsync(ct);
    }

    public async Task<int> CountAsync(CancellationToken ct) =>
        await _context.Categories.CountAsync(ct);

    public async Task<Category?> GetByIdAsync(int id, CancellationToken ct)
    {
        return await _context.Categories
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.Id == id, ct);
    }

    public async Task AddAsync(Category category) =>
        await _context.Categories.AddAsync(category);

    public void Update(Category category) => _context.Categories.Update(category);

    public void Remove(Category category) => _context.Categories.Remove(category);

    public async Task SaveChangesAsync() =>
        await _context.SaveChangesAsync();
}
