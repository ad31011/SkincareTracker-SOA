using Microsoft.EntityFrameworkCore;
using SkincareTracker.API.Data;
using SkincareTracker.API.Interfaces;
using SkincareTracker.API.Models;

namespace SkincareTracker.API.Repositories;

// ── User Repository ───────────────────────────────────────────────────────────
public class UserRepository : IUserRepository
{
    private readonly AppDbContext _db;
    public UserRepository(AppDbContext db) => _db = db;

    public async Task<User?> GetByIdAsync(int id) =>
        await _db.Users.FindAsync(id);

    public async Task<User?> GetByEmailAsync(string email) =>
        await _db.Users.FirstOrDefaultAsync(u => u.Email == email);

    public async Task<IEnumerable<User>> GetAllAsync() =>
        await _db.Users.ToListAsync();

    public async Task<User> CreateAsync(User user)
    {
        Console.WriteLine($"Name={user.Name}");
        Console.WriteLine($"SkinType={user.SkinType}");
        Console.WriteLine($"SkinConcerns={user.SkinConcerns ?? "NULL"}");

        _db.Users.Add(user);

        var entry = _db.Entry(user);
        foreach (var p in entry.Properties)
        {
            Console.WriteLine($"{p.Metadata.Name} = {p.CurrentValue ?? "NULL"}");
        }

        await _db.SaveChangesAsync();
        return user;
    }

    public async Task<User> UpdateAsync(User user)
    {
        _db.Users.Update(user);
        await _db.SaveChangesAsync();
        return user;
    }

    public async Task DeleteAsync(int id)
    {
        var user = await _db.Users.FindAsync(id);
        if (user != null) { _db.Users.Remove(user); await _db.SaveChangesAsync(); }
    }
}

// ── Product Repository ────────────────────────────────────────────────────────
public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _db;
    public ProductRepository(AppDbContext db) => _db = db;

    public async Task<Product?> GetByIdAsync(int id) =>
        await _db.Products
            .Include(p => p.ProductIngredients).ThenInclude(pi => pi.Ingredient)
            .FirstOrDefaultAsync(p => p.Id == id);

    public async Task<IEnumerable<Product>> GetAllAsync() =>
        await _db.Products
            .Include(p => p.ProductIngredients).ThenInclude(pi => pi.Ingredient)
            .ToListAsync();

    public async Task<Product> CreateAsync(Product product)
    {
        _db.Products.Add(product);
        await _db.SaveChangesAsync();
        return product;
    }

    public async Task<Product> UpdateAsync(Product product)
    {
        _db.Products.Update(product);
        await _db.SaveChangesAsync();
        return product;
    }

    public async Task DeleteAsync(int id)
    {
        var product = await _db.Products.FindAsync(id);
        if (product != null) { _db.Products.Remove(product); await _db.SaveChangesAsync(); }
    }

    public async Task AddIngredientAsync(int productId, int ingredientId)
    {
        _db.ProductIngredients.Add(new ProductIngredient { ProductId = productId, IngredientId = ingredientId });
        await _db.SaveChangesAsync();
    }

    public async Task RemoveAllIngredientsAsync(int productId)
    {
        var links = _db.ProductIngredients.Where(pi => pi.ProductId == productId);
        _db.ProductIngredients.RemoveRange(links);
        await _db.SaveChangesAsync();
    }
}

// ── Ingredient Repository ─────────────────────────────────────────────────────
public class IngredientRepository : IIngredientRepository
{
    private readonly AppDbContext _db;
    public IngredientRepository(AppDbContext db) => _db = db;

    public async Task<Ingredient?> GetByIdAsync(int id) => await _db.Ingredients.FindAsync(id);
    public async Task<IEnumerable<Ingredient>> GetAllAsync() => await _db.Ingredients.ToListAsync();

    public async Task<Ingredient> CreateAsync(Ingredient ingredient)
    {
        _db.Ingredients.Add(ingredient);
        await _db.SaveChangesAsync();
        return ingredient;
    }

    public async Task<Ingredient> UpdateAsync(Ingredient ingredient)
    {
        _db.Ingredients.Update(ingredient);
        await _db.SaveChangesAsync();
        return ingredient;
    }

    public async Task DeleteAsync(int id)
    {
        var i = await _db.Ingredients.FindAsync(id);
        if (i != null) { _db.Ingredients.Remove(i); await _db.SaveChangesAsync(); }
    }
}

// ── Routine Repository ────────────────────────────────────────────────────────
public class RoutineRepository : IRoutineRepository
{
    private readonly AppDbContext _db;
    public RoutineRepository(AppDbContext db) => _db = db;

    public async Task<Routine?> GetByIdAsync(int id) =>
        await _db.Routines
            .Include(r => r.RoutineProducts).ThenInclude(rp => rp.Product)
            .FirstOrDefaultAsync(r => r.Id == id);

    public async Task<IEnumerable<Routine>> GetByUserIdAsync(int userId) =>
        await _db.Routines
            .Where(r => r.UserId == userId)
            .Include(r => r.RoutineProducts).ThenInclude(rp => rp.Product)
            .ToListAsync();

    public async Task<Routine> CreateAsync(Routine routine)
    {
        _db.Routines.Add(routine);
        await _db.SaveChangesAsync();
        return routine;
    }

    public async Task<Routine> UpdateAsync(Routine routine)
    {
        _db.Routines.Update(routine);
        await _db.SaveChangesAsync();
        return routine;
    }

    public async Task DeleteAsync(int id)
    {
        var r = await _db.Routines.FindAsync(id);
        if (r != null) { _db.Routines.Remove(r); await _db.SaveChangesAsync(); }
    }

    public async Task AddProductAsync(int routineId, int productId, int stepOrder)
    {
        _db.RoutineProducts.Add(new RoutineProduct { RoutineId = routineId, ProductId = productId, StepOrder = stepOrder });
        await _db.SaveChangesAsync();
    }

    public async Task RemoveAllProductsAsync(int routineId)
    {
        var links = _db.RoutineProducts.Where(rp => rp.RoutineId == routineId);
        _db.RoutineProducts.RemoveRange(links);
        await _db.SaveChangesAsync();
    }
}

// ── SkinLog Repository ────────────────────────────────────────────────────────
public class SkinLogRepository : ISkinLogRepository
{
    private readonly AppDbContext _db;
    public SkinLogRepository(AppDbContext db) => _db = db;

    public async Task<SkinLog?> GetByIdAsync(int id) => await _db.SkinLogs.FindAsync(id);

    public async Task<IEnumerable<SkinLog>> GetByUserIdAsync(int userId) =>
        await _db.SkinLogs.Where(sl => sl.UserId == userId).OrderByDescending(sl => sl.Date).ToListAsync();

    public async Task<IEnumerable<SkinLog>> GetByUserIdAndRangeAsync(int userId, DateTime from, DateTime to) =>
        await _db.SkinLogs
            .Where(sl => sl.UserId == userId && sl.Date >= from && sl.Date <= to)
            .OrderBy(sl => sl.Date)
            .ToListAsync();

    public async Task<SkinLog> CreateAsync(SkinLog log)
    {
        _db.SkinLogs.Add(log);
        await _db.SaveChangesAsync();
        return log;
    }

    public async Task<SkinLog> UpdateAsync(SkinLog log)
    {
        _db.SkinLogs.Update(log);
        await _db.SaveChangesAsync();
        return log;
    }

    public async Task DeleteAsync(int id)
    {
        var log = await _db.SkinLogs.FindAsync(id);
        if (log != null) { _db.SkinLogs.Remove(log); await _db.SaveChangesAsync(); }
    }
}
