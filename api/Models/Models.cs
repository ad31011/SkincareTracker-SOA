namespace SkincareTracker.API.Models;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = "User"; // "User" | "Admin"
    public string SkinType { get; set; } = string.Empty;  // Oily, Dry, Combination, Normal, Sensitive
    public string SkinConcerns { get; set; } = string.Empty; // comma-separated
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Routine> Routines { get; set; } = new List<Routine>();
    public ICollection<SkinLog> SkinLogs { get; set; } = new List<SkinLog>();
}

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty; // Cleanser, Toner, Serum, Moisturizer, SPF, Treatment, Eye, Mask
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<ProductIngredient> ProductIngredients { get; set; } = new List<ProductIngredient>();
    public ICollection<RoutineProduct> RoutineProducts { get; set; } = new List<RoutineProduct>();
}

public class Ingredient
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ConflictsWith { get; set; }  // comma-separated ingredient names it conflicts with
    public string? Notes { get; set; }

    public ICollection<ProductIngredient> ProductIngredients { get; set; } = new List<ProductIngredient>();
}

public class ProductIngredient
{
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public int IngredientId { get; set; }
    public Ingredient Ingredient { get; set; } = null!;
}

public class Routine
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public string Name { get; set; } = string.Empty;
    public string TimeOfDay { get; set; } = string.Empty;  // AM | PM
    public string DaysOfWeek { get; set; } = string.Empty; // e.g. "Mon,Wed,Fri"
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<RoutineProduct> RoutineProducts { get; set; } = new List<RoutineProduct>();
}

public class RoutineProduct
{
    public int RoutineId { get; set; }
    public Routine Routine { get; set; } = null!;
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public int StepOrder { get; set; }
}

public class SkinLog
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public int SkinConditionScore { get; set; } // 1–10
    public string Hydration { get; set; } = string.Empty;   // Low | Medium | High
    public string Oiliness { get; set; } = string.Empty;    // Low | Medium | High
    public string Sensitivity { get; set; } = string.Empty; // Low | Medium | High
    public string? Notes { get; set; }
    public string ProductsUsed { get; set; } = string.Empty; // comma-separated product IDs
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
