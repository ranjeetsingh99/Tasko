using Microsoft.EntityFrameworkCore;

using Tasko.Data;
using Tasko.Models;

namespace Tasko.Services;

public class TodoService
{
    private readonly ApplicationDbContext _db;
    public TodoService(ApplicationDbContext db) => _db = db;

    //soft delete
    public async Task SoftDeleteAsync(int id,string ownerId)
    {
        var todo = await _db.Todos.FirstOrDefaultAsync(t => t.Id == id && t.OwnerId == ownerId);
        if (todo == null) return;
        todo.IsDeleted = true;
        todo.DeletedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
    }

    // for trash screen
    public Task<List<TodoItem>> GetTrashAsync(string ownerId) =>
        _db.Todos.IgnoreQueryFilters()
                    .Where(t => t.OwnerId == ownerId && t.IsDeleted)
                    .OrderByDescending(t => t.DeletedAt)
                    .ToListAsync();
    public async Task RestoreAsync(int id,string ownerId)
    {
        var todo = await _db.Todos.IgnoreQueryFilters()
            .FirstOrDefaultAsync(t => t.Id == id && t.OwnerId == ownerId && t.IsDeleted);
        if(todo == null) return;
        todo.IsDeleted = false;
        todo.DeletedAt = null;
        await _db.SaveChangesAsync();
    }

    //Hard Delete

    public async Task DeleteForeverAsync(int id, string ownerId)
    {
        await _db.Todos.IgnoreQueryFilters()
            .Where(t => t.Id == id && t.OwnerId == ownerId && t.IsDeleted)
            .ExecuteDeleteAsync();
    }
    public async Task DeleteCategoryAsync(int id, string ownerId)
    {
        await using var tx = await _db.Database.BeginTransactionAsync();
        await _db.Todos.IgnoreQueryFilters()
            .Where(t => t.CategoryId == id && t.OwnerId == ownerId)
            .ExecuteUpdateAsync(s => s.SetProperty(t => t.CategoryId, (int?)null));
        await _db.Categories
            .Where(c => c.Id == id && c.OwnerId == ownerId)
            .ExecuteDeleteAsync();

        await tx.CommitAsync();
    }
}
