using SkincareTracker.API.DTOs;
using SkincareTracker.API.Helpers;
using SkincareTracker.API.Interfaces;
using SkincareTracker.API.Models;

namespace SkincareTracker.API.Services;

// ── Auth Service ──────────────────────────────────────────────────────────────
public class AuthService : IAuthService
{
    private readonly IUserRepository _users;
    private readonly IConfiguration _config;

    public AuthService(IUserRepository users, IConfiguration config)
    {
        _users = users;
        _config = config;
    }

    public async Task<bool> RegisterAsync(string name, string email, string password, string skinType, string skinConcerns)
    {
        if (await _users.GetByEmailAsync(email) != null) return false;

        var user = new User
        {
            Name         = name,
            Email        = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            SkinType     = skinType,
            SkinConcerns = skinConcerns,
            Role         = "User"
        };
        await _users.CreateAsync(user);
        return true;
    }

    public async Task<AuthResponseDto?> LoginAsync(string email, string password)
    {
        var user = await _users.GetByEmailAsync(email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            return null;

        var key   = _config["Jwt:Key"] ?? "SuperSecretSkincareTrackerKey2024!XYZ";
        var token = JwtHelper.GenerateToken(user.Id, user.Email, user.Role, key);
        return new AuthResponseDto(token, user.Name, user.Email, user.Role, user.Id);
    }

    public async Task<UserDto?> GetCurrentUserAsync(int userId)
    {
        var user = await _users.GetByIdAsync(userId);
        if (user == null) return null;
        return new UserDto(user.Id, user.Name, user.Email, user.Role, user.SkinType, user.SkinConcerns, user.CreatedAt);
    }
}

// ── User Service ──────────────────────────────────────────────────────────────
public class UserService : IUserService
{
    private readonly IUserRepository _users;

    public UserService(IUserRepository users) => _users = users;

    public async Task<IEnumerable<UserDto>> GetAllAsync()
    {
        var users = await _users.GetAllAsync();
        return users.Select(u => new UserDto(
            u.Id, u.Name, u.Email, u.Role,
            u.SkinType, u.SkinConcerns, u.CreatedAt));
    }

    public async Task DeleteAsync(int id)
    {
        await _users.DeleteAsync(id);
    }
}

// ── Product Service ───────────────────────────────────────────────────────────
public class ProductService : IProductService
{
    private readonly IProductRepository _repo;

    public ProductService(IProductRepository repo) => _repo = repo;

    public async Task<IEnumerable<ProductDto>> GetAllAsync()
    {
        var products = await _repo.GetAllAsync();
        return products.Select(ToDto);
    }

    public async Task<ProductDto?> GetByIdAsync(int id)
    {
        var p = await _repo.GetByIdAsync(id);
        return p == null ? null : ToDto(p);
    }

    public async Task<ProductDto> CreateAsync(CreateProductDto dto)
    {
        var product = new Product
        {
            Name        = dto.Name,
            Brand       = dto.Brand,
            Category    = dto.Category,
            Description = dto.Description
        };
        var created = await _repo.CreateAsync(product);
        foreach (var ingId in dto.IngredientIds)
            await _repo.AddIngredientAsync(created.Id, ingId);
        return ToDto((await _repo.GetByIdAsync(created.Id))!);
    }

    public async Task<ProductDto?> UpdateAsync(int id, UpdateProductDto dto)
    {
        var product = await _repo.GetByIdAsync(id);
        if (product == null) return null;
        product.Name        = dto.Name;
        product.Brand       = dto.Brand;
        product.Category    = dto.Category;
        product.Description = dto.Description;
        await _repo.UpdateAsync(product);
        await _repo.RemoveAllIngredientsAsync(id);
        foreach (var ingId in dto.IngredientIds)
            await _repo.AddIngredientAsync(id, ingId);
        return ToDto((await _repo.GetByIdAsync(id))!);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var p = await _repo.GetByIdAsync(id);
        if (p == null) return false;
        await _repo.DeleteAsync(id);
        return true;
    }

    private static ProductDto ToDto(Product p) => new(
        p.Id, p.Name, p.Brand, p.Category, p.Description,
        p.ProductIngredients.Select(pi => pi.Ingredient.Name).ToList());
}

// ── Ingredient Service ────────────────────────────────────────────────────────
public class IngredientService : IIngredientService
{
    private readonly IIngredientRepository _repo;

    public IngredientService(IIngredientRepository repo) => _repo = repo;

    public async Task<IEnumerable<IngredientDto>> GetAllAsync()
    {
        var list = await _repo.GetAllAsync();
        return list.Select(i => new IngredientDto(i.Id, i.Name, i.ConflictsWith, i.Notes));
    }

    public async Task<IngredientDto?> GetByIdAsync(int id)
    {
        var i = await _repo.GetByIdAsync(id);
        return i == null ? null : new IngredientDto(i.Id, i.Name, i.ConflictsWith, i.Notes);
    }

    public async Task<IngredientDto> CreateAsync(CreateIngredientDto dto)
    {
        var i       = new Ingredient { Name = dto.Name, ConflictsWith = dto.ConflictsWith, Notes = dto.Notes };
        var created = await _repo.CreateAsync(i);
        return new IngredientDto(created.Id, created.Name, created.ConflictsWith, created.Notes);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var i = await _repo.GetByIdAsync(id);
        if (i == null) return false;
        await _repo.DeleteAsync(id);
        return true;
    }
}

// ── Routine Service ───────────────────────────────────────────────────────────
public class RoutineService : IRoutineService
{
    private readonly IRoutineRepository    _repo;
    private readonly IIngredientRepository _ingredients;
    private readonly IProductRepository    _products;

    public RoutineService(IRoutineRepository repo, IIngredientRepository ingredients, IProductRepository products)
    {
        _repo        = repo;
        _ingredients = ingredients;
        _products    = products;
    }

    public async Task<IEnumerable<RoutineDto>> GetByUserAsync(int userId)
    {
        var routines = await _repo.GetByUserIdAsync(userId);
        return routines.Select(ToDto);
    }

    public async Task<RoutineDto?> GetByIdAsync(int id)
    {
        var r = await _repo.GetByIdAsync(id);
        return r == null ? null : ToDto(r);
    }

    public async Task<RoutineDto> CreateAsync(int userId, CreateRoutineDto dto)
    {
        var routine = new Routine
        {
            UserId     = userId,
            Name       = dto.Name,
            TimeOfDay  = dto.TimeOfDay,
            DaysOfWeek = dto.DaysOfWeek
        };
        var created = await _repo.CreateAsync(routine);
        foreach (var p in dto.Products)
            await _repo.AddProductAsync(created.Id, p.ProductId, p.StepOrder);
        return ToDto((await _repo.GetByIdAsync(created.Id))!);
    }

    public async Task<RoutineDto?> UpdateAsync(int id, int userId, UpdateRoutineDto dto)
    {
        var routine = await _repo.GetByIdAsync(id);
        if (routine == null || routine.UserId != userId) return null;
        routine.Name       = dto.Name;
        routine.TimeOfDay  = dto.TimeOfDay;
        routine.DaysOfWeek = dto.DaysOfWeek;
        routine.IsActive   = dto.IsActive;
        await _repo.UpdateAsync(routine);
        await _repo.RemoveAllProductsAsync(id);
        foreach (var p in dto.Products)
            await _repo.AddProductAsync(id, p.ProductId, p.StepOrder);
        return ToDto((await _repo.GetByIdAsync(id))!);
    }

    public async Task<bool> DeleteAsync(int id, int userId)
    {
        var r = await _repo.GetByIdAsync(id);
        if (r == null || r.UserId != userId) return false;
        await _repo.DeleteAsync(id);
        return true;
    }

    /// <summary>
    /// Business Logic: Checks all products in a routine for ingredient conflicts.
    /// E.g. if Retinol and AHA are both in the routine, it flags them.
    /// </summary>
    public async Task<List<ConflictWarningDto>> CheckConflictsAsync(int routineId)
    {
        var routine = await _repo.GetByIdAsync(routineId);
        if (routine == null) return new();

        var warnings = new List<ConflictWarningDto>();
        var products = routine.RoutineProducts.Select(rp => rp.Product).ToList();

        for (int i = 0; i < products.Count; i++)
        {
            for (int j = i + 1; j < products.Count; j++)
            {
                var p1Ingredients = products[i].ProductIngredients.Select(pi => pi.Ingredient).ToList();
                var p2Ingredients = products[j].ProductIngredients.Select(pi => pi.Ingredient).ToList();

                foreach (var ing1 in p1Ingredients)
                {
                    if (string.IsNullOrEmpty(ing1.ConflictsWith)) continue;
                    var conflicts = ing1.ConflictsWith.Split(',').Select(c => c.Trim()).ToList();

                    foreach (var ing2 in p2Ingredients)
                    {
                        if (conflicts.Contains(ing2.Name, StringComparer.OrdinalIgnoreCase))
                        {
                            warnings.Add(new ConflictWarningDto(
                                products[i].Name, products[j].Name, ing1.Name,
                                $"'{ing1.Name}' in {products[i].Name} conflicts with '{ing2.Name}' in {products[j].Name}. Consider using them in separate routines."));
                        }
                    }
                }
            }
        }
        return warnings;
    }

    private static RoutineDto ToDto(Routine r) => new(
        r.Id, r.Name, r.TimeOfDay, r.DaysOfWeek, r.IsActive,
        r.RoutineProducts.OrderBy(rp => rp.StepOrder)
            .Select(rp => new RoutineProductDto(
                rp.ProductId, rp.Product.Name, rp.Product.Brand,
                rp.Product.Category, rp.StepOrder))
            .ToList());
}

// ── SkinLog Service ───────────────────────────────────────────────────────────
public class SkinLogService : ISkinLogService
{
    private readonly ISkinLogRepository _repo;

    public SkinLogService(ISkinLogRepository repo) => _repo = repo;

    public async Task<IEnumerable<SkinLogDto>> GetByUserAsync(int userId)
    {
        var logs = await _repo.GetByUserIdAsync(userId);
        return logs.Select(ToDto);
    }

    public async Task<SkinLogDto?> GetByIdAsync(int id)
    {
        var log = await _repo.GetByIdAsync(id);
        return log == null ? null : ToDto(log);
    }

    public async Task<SkinLogDto> CreateAsync(int userId, CreateSkinLogDto dto)
    {
        var log = new SkinLog
        {
            UserId             = userId,
            Date               = dto.Date,
            SkinConditionScore = dto.SkinConditionScore,
            Hydration          = dto.Hydration,
            Oiliness           = dto.Oiliness,
            Sensitivity        = dto.Sensitivity,
            Notes              = dto.Notes,
            ProductsUsed       = dto.ProductsUsed
        };
        var created = await _repo.CreateAsync(log);
        return ToDto(created);
    }

    public async Task<SkinLogDto?> UpdateAsync(int id, int userId, UpdateSkinLogDto dto)
    {
        var log = await _repo.GetByIdAsync(id);
        if (log == null || log.UserId != userId) return null;
        log.SkinConditionScore = dto.SkinConditionScore;
        log.Hydration          = dto.Hydration;
        log.Oiliness           = dto.Oiliness;
        log.Sensitivity        = dto.Sensitivity;
        log.Notes              = dto.Notes;
        log.ProductsUsed       = dto.ProductsUsed;
        return ToDto(await _repo.UpdateAsync(log));
    }

    public async Task<bool> DeleteAsync(int id, int userId)
    {
        var log = await _repo.GetByIdAsync(id);
        if (log == null || log.UserId != userId) return false;
        await _repo.DeleteAsync(id);
        return true;
    }

    /// <summary>
    /// Business Logic: Returns daily skin scores for a date range (for progress chart).
    /// </summary>
    public async Task<List<SkinProgressDto>> GetProgressAsync(int userId, DateTime from, DateTime to)
    {
        var logs = await _repo.GetByUserIdAndRangeAsync(userId, from, to);
        return logs.Select(l => new SkinProgressDto(
            l.Date, l.SkinConditionScore, l.Hydration, l.Oiliness, l.Sensitivity)).ToList();
    }

    /// <summary>
    /// Business Logic: Calculates current and longest logging streaks.
    /// A streak is consecutive days where at least one log exists.
    /// </summary>
    public async Task<StreakDto> GetStreakAsync(int userId)
    {
        var logs = await _repo.GetByUserIdAsync(userId);
        var days = logs.Select(l => l.Date.Date).Distinct().OrderBy(d => d).ToList();

        if (!days.Any()) return new StreakDto(0, 0, 0);

        int longest = 1, streak = 1;
        var today = DateTime.UtcNow.Date;

        for (int i = 1; i < days.Count; i++)
        {
            streak = (days[i] - days[i - 1]).TotalDays == 1 ? streak + 1 : 1;
            if (streak > longest) longest = streak;
        }

        int current = (today - days.Last()).TotalDays <= 1 ? streak : 0;
        return new StreakDto(current, longest, days.Count);
    }

    private static SkinLogDto ToDto(SkinLog l) => new(
        l.Id, l.Date, l.SkinConditionScore,
        l.Hydration, l.Oiliness, l.Sensitivity, l.Notes, l.ProductsUsed);
}
