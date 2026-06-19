using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkincareTracker.API.DTOs;
using SkincareTracker.API.Interfaces;
using System.Security.Claims;

namespace SkincareTracker.API.Controllers;

// ── Auth Controller ───────────────────────────────────────────────────────────
// Injects ONLY: IAuthService
[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;

    public AuthController(IAuthService auth) => _auth = auth;

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var ok = await _auth.RegisterAsync(dto.Name, dto.Email, dto.Password, dto.SkinType, dto.SkinConcerns);
        if (!ok) return BadRequest(new { message = "Email already in use." });
        return Ok(new { message = "Account created." });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var result = await _auth.LoginAsync(dto.Email, dto.Password);
        if (result == null) return Unauthorized(new { message = "Invalid email or password." });
        return Ok(result);
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> Me()
    {
        var userId = GetUserId();
        if (userId == 0) return Unauthorized();
        var user = await _auth.GetCurrentUserAsync(userId);
        return user == null ? NotFound() : Ok(user);
    }

    private int GetUserId()
    {
        var uid = User.FindFirstValue("uid")
               ?? User.FindFirstValue(ClaimTypes.NameIdentifier)
               ?? "0";
        return int.TryParse(uid, out var id) ? id : 0;
    }
}

// ── Products Controller ───────────────────────────────────────────────────────
// Injects ONLY: IProductService
[ApiController]
[Route("api/products")]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly IProductService _products;

    public ProductsController(IProductService products) => _products = products;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _products.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var p = await _products.GetByIdAsync(id);
        return p == null ? NotFound() : Ok(p);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
        => Ok(await _products.CreateAsync(dto));

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProductDto dto)
    {
        var result = await _products.UpdateAsync(id, dto);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var ok = await _products.DeleteAsync(id);
        return ok ? Ok() : NotFound();
    }
}

// ── Ingredients Controller ────────────────────────────────────────────────────
// Injects ONLY: IIngredientService
[ApiController]
[Route("api/ingredients")]
[Authorize]
public class IngredientsController : ControllerBase
{
    private readonly IIngredientService _ingredients;

    public IngredientsController(IIngredientService ingredients) => _ingredients = ingredients;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _ingredients.GetAllAsync());

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateIngredientDto dto)
        => Ok(await _ingredients.CreateAsync(dto));

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var ok = await _ingredients.DeleteAsync(id);
        return ok ? Ok() : NotFound();
    }
}

// ── Routines Controller ───────────────────────────────────────────────────────
// Injects ONLY: IRoutineService
[ApiController]
[Route("api/routines")]
[Authorize]
public class RoutinesController : ControllerBase
{
    private readonly IRoutineService _routines;

    public RoutinesController(IRoutineService routines) => _routines = routines;

    private int GetUserId()
    {
        var uid = User.FindFirstValue("uid")
               ?? User.FindFirstValue(ClaimTypes.NameIdentifier)
               ?? "0";
        return int.TryParse(uid, out var id) ? id : 0;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(await _routines.GetByUserAsync(GetUserId()));

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var r = await _routines.GetByIdAsync(id);
        return r == null ? NotFound() : Ok(r);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRoutineDto dto)
        => Ok(await _routines.CreateAsync(GetUserId(), dto));

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateRoutineDto dto)
    {
        var result = await _routines.UpdateAsync(id, GetUserId(), dto);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var ok = await _routines.DeleteAsync(id, GetUserId());
        return ok ? Ok() : NotFound();
    }

    [HttpGet("{id}/conflicts")]
    public async Task<IActionResult> Conflicts(int id)
        => Ok(await _routines.CheckConflictsAsync(id));
}

// ── SkinLogs Controller ───────────────────────────────────────────────────────
// Injects ONLY: ISkinLogService
[ApiController]
[Route("api/skinlogs")]
[Authorize]
public class SkinLogsController : ControllerBase
{
    private readonly ISkinLogService _skinLogs;

    public SkinLogsController(ISkinLogService skinLogs) => _skinLogs = skinLogs;

    private int GetUserId()
    {
        var uid = User.FindFirstValue("uid")
               ?? User.FindFirstValue(ClaimTypes.NameIdentifier)
               ?? "0";
        return int.TryParse(uid, out var id) ? id : 0;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(await _skinLogs.GetByUserAsync(GetUserId()));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSkinLogDto dto)
        => Ok(await _skinLogs.CreateAsync(GetUserId(), dto));

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateSkinLogDto dto)
    {
        var result = await _skinLogs.UpdateAsync(id, GetUserId(), dto);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var ok = await _skinLogs.DeleteAsync(id, GetUserId());
        return ok ? Ok() : NotFound();
    }

    [HttpGet("streak")]
    public async Task<IActionResult> Streak()
        => Ok(await _skinLogs.GetStreakAsync(GetUserId()));

    [HttpGet("progress")]
    public async Task<IActionResult> Progress([FromQuery] string? from, [FromQuery] string? to)
    {
        var fromDt = from != null ? DateTime.Parse(from).ToUniversalTime() : DateTime.UtcNow.AddDays(-29);
        var toDt   = to   != null ? DateTime.Parse(to).ToUniversalTime()   : DateTime.UtcNow;
        return Ok(await _skinLogs.GetProgressAsync(GetUserId(), fromDt, toDt));
    }
}

// ── Users Controller (Admin) ──────────────────────────────────────────────────
// Injects ONLY: IUserService
[ApiController]
[Route("api/users")]
[Authorize(Roles = "Admin")]
public class UsersController : ControllerBase
{
    private readonly IUserService _users;

    public UsersController(IUserService users) => _users = users;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _users.GetAllAsync());

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _users.DeleteAsync(id);
        return Ok();
    }
}
