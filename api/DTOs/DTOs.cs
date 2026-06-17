namespace SkincareTracker.API.DTOs;

// ── Auth ──────────────────────────────────────────────────────────────────────
public record RegisterDto(string Name, string Email, string Password, string SkinType, string SkinConcerns);
public record LoginDto(string Email, string Password);
public record AuthResponseDto(string Token, string Name, string Email, string Role, int UserId);

// ── User ──────────────────────────────────────────────────────────────────────
public record UserDto(int Id, string Name, string Email, string Role, string SkinType, string SkinConcerns, DateTime CreatedAt);
public record UpdateUserDto(string Name, string SkinType, string SkinConcerns);

// ── Product ───────────────────────────────────────────────────────────────────
public record ProductDto(int Id, string Name, string Brand, string Category, string Description, List<string> Ingredients);
public record CreateProductDto(string Name, string Brand, string Category, string Description, List<int> IngredientIds);
public record UpdateProductDto(string Name, string Brand, string Category, string Description, List<int> IngredientIds);

// ── Ingredient ────────────────────────────────────────────────────────────────
public record IngredientDto(int Id, string Name, string? ConflictsWith, string? Notes);
public record CreateIngredientDto(string Name, string? ConflictsWith, string? Notes);

// ── Routine ───────────────────────────────────────────────────────────────────
public record RoutineDto(int Id, string Name, string TimeOfDay, string DaysOfWeek, bool IsActive, List<RoutineProductDto> Products);
public record RoutineProductDto(int ProductId, string ProductName, string Brand, string Category, int StepOrder);
public record CreateRoutineDto(string Name, string TimeOfDay, string DaysOfWeek, List<RoutineProductStepDto> Products);
public record UpdateRoutineDto(string Name, string TimeOfDay, string DaysOfWeek, bool IsActive, List<RoutineProductStepDto> Products);
public record RoutineProductStepDto(int ProductId, int StepOrder);

// ── SkinLog ───────────────────────────────────────────────────────────────────
public record SkinLogDto(int Id, DateTime Date, int SkinConditionScore, string Hydration, string Oiliness, string Sensitivity, string? Notes, string ProductsUsed);
public record CreateSkinLogDto(DateTime Date, int SkinConditionScore, string Hydration, string Oiliness, string Sensitivity, string? Notes, string ProductsUsed);
public record UpdateSkinLogDto(int SkinConditionScore, string Hydration, string Oiliness, string Sensitivity, string? Notes, string ProductsUsed);

// ── Analytics ─────────────────────────────────────────────────────────────────
public record SkinProgressDto(DateTime Date, int Score, string Hydration, string Oiliness, string Sensitivity);
public record StreakDto(int CurrentStreak, int LongestStreak, int TotalLogs);
public record ConflictWarningDto(string Product1, string Product2, string ConflictingIngredient, string Message);
public record RoutineAdherenceDto(string RoutineName, int ExpectedDays, int LoggedDays, double AdherencePercent);
