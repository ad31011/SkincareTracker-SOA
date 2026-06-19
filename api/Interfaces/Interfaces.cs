using SkincareTracker.API.Models;

namespace SkincareTracker.API.Interfaces;

// ─────────────────────────────────────────────────────────────────────────────
// REPOSITORY INTERFACES
// Used ONLY by Services — Controllers must never inject these directly.
// ─────────────────────────────────────────────────────────────────────────────

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int id);
    Task<User?> GetByEmailAsync(string email);
    Task<IEnumerable<User>> GetAllAsync();
    Task<User> CreateAsync(User user);
    Task<User> UpdateAsync(User user);
    Task DeleteAsync(int id);
}

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(int id);
    Task<IEnumerable<Product>> GetAllAsync();
    Task<Product> CreateAsync(Product product);
    Task<Product> UpdateAsync(Product product);
    Task DeleteAsync(int id);
    Task AddIngredientAsync(int productId, int ingredientId);
    Task RemoveAllIngredientsAsync(int productId);
}

public interface IIngredientRepository
{
    Task<Ingredient?> GetByIdAsync(int id);
    Task<IEnumerable<Ingredient>> GetAllAsync();
    Task<Ingredient> CreateAsync(Ingredient ingredient);
    Task<Ingredient> UpdateAsync(Ingredient ingredient);
    Task DeleteAsync(int id);
}

public interface IRoutineRepository
{
    Task<Routine?> GetByIdAsync(int id);
    Task<IEnumerable<Routine>> GetByUserIdAsync(int userId);
    Task<Routine> CreateAsync(Routine routine);
    Task<Routine> UpdateAsync(Routine routine);
    Task DeleteAsync(int id);
    Task AddProductAsync(int routineId, int productId, int stepOrder);
    Task RemoveAllProductsAsync(int routineId);
}

public interface ISkinLogRepository
{
    Task<SkinLog?> GetByIdAsync(int id);
    Task<IEnumerable<SkinLog>> GetByUserIdAsync(int userId);
    Task<IEnumerable<SkinLog>> GetByUserIdAndRangeAsync(int userId, DateTime from, DateTime to);
    Task<SkinLog> CreateAsync(SkinLog log);
    Task<SkinLog> UpdateAsync(SkinLog log);
    Task DeleteAsync(int id);
}

// ─────────────────────────────────────────────────────────────────────────────
// SERVICE INTERFACES
// These are the ONLY interfaces that Controllers may inject.
// ─────────────────────────────────────────────────────────────────────────────

public interface IAuthService
{
    Task<bool> RegisterAsync(string name, string email, string password, string skinType, string skinConcerns);
    Task<DTOs.AuthResponseDto?> LoginAsync(string email, string password);
    Task<DTOs.UserDto?> GetCurrentUserAsync(int userId);
}

public interface IUserService
{
    Task<IEnumerable<DTOs.UserDto>> GetAllAsync();
    Task DeleteAsync(int id);
}

public interface IProductService
{
    Task<IEnumerable<DTOs.ProductDto>> GetAllAsync();
    Task<DTOs.ProductDto?> GetByIdAsync(int id);
    Task<DTOs.ProductDto> CreateAsync(DTOs.CreateProductDto dto);
    Task<DTOs.ProductDto?> UpdateAsync(int id, DTOs.UpdateProductDto dto);
    Task<bool> DeleteAsync(int id);
}

public interface IIngredientService
{
    Task<IEnumerable<DTOs.IngredientDto>> GetAllAsync();
    Task<DTOs.IngredientDto?> GetByIdAsync(int id);
    Task<DTOs.IngredientDto> CreateAsync(DTOs.CreateIngredientDto dto);
    Task<bool> DeleteAsync(int id);
}

public interface IRoutineService
{
    Task<IEnumerable<DTOs.RoutineDto>> GetByUserAsync(int userId);
    Task<DTOs.RoutineDto?> GetByIdAsync(int id);
    Task<DTOs.RoutineDto> CreateAsync(int userId, DTOs.CreateRoutineDto dto);
    Task<DTOs.RoutineDto?> UpdateAsync(int id, int userId, DTOs.UpdateRoutineDto dto);
    Task<bool> DeleteAsync(int id, int userId);
    Task<List<DTOs.ConflictWarningDto>> CheckConflictsAsync(int routineId);
}

public interface ISkinLogService
{
    Task<IEnumerable<DTOs.SkinLogDto>> GetByUserAsync(int userId);
    Task<DTOs.SkinLogDto?> GetByIdAsync(int id);
    Task<DTOs.SkinLogDto> CreateAsync(int userId, DTOs.CreateSkinLogDto dto);
    Task<DTOs.SkinLogDto?> UpdateAsync(int id, int userId, DTOs.UpdateSkinLogDto dto);
    Task<bool> DeleteAsync(int id, int userId);
    Task<List<DTOs.SkinProgressDto>> GetProgressAsync(int userId, DateTime from, DateTime to);
    Task<DTOs.StreakDto> GetStreakAsync(int userId);
}
