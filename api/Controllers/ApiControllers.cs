using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkincareTracker.API.DTOs;
using SkincareTracker.API.Interfaces;
using System.Security.Claims;

namespace SkincareTracker.API.Controllers;

// ── Auth ──────────────────────────────────────────────────────────────────────
[ApiController]
[Route("api/auth")]
public class AuthApiController : ControllerBase
{
    private readonly IAuthService _auth;
    private readonly IUserRepository _users;
    public AuthApiController(IAuthService auth, IUserRepository users) { _auth = auth; _users = users; }

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
        var token = await _auth.AuthenticateAsync(dto.Email, dto.Password);
        if (token == null) return Unauthorized(new { message = "Invalid email or password." });
        var user = await _users.GetByEmailAsync(dto.Email);
        return Ok(new AuthResponseDto(token, user!.Name, user.Email, user.Role, user.Id));
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> Me()
    {
        var id = GetUserId();
        if (id == 0) return Unauthorized();
        var user = await _users.GetByIdAsync(id);
        if (user == null) return NotFound();
        return Ok(new UserDto(user.Id, user.Name, user.Email, user.Role, user.SkinType, user.SkinConcerns, user.CreatedAt));
    }

    private int GetUserId()
    {
        var uid = User.FindFirstValue("uid")
               ?? User.FindFirstValue(ClaimTypes.NameIdentifier)
               ?? "0";
        return int.TryParse(uid, out var id) ? id : 0;
    }
}

// ── Products ──────────────────────────────────────────────────────────────────
[ApiController]
[Route("api/products")]
[Authorize]
public class ProductsApiController : ControllerBase
{
    private readonly IProductService _products;
    private readonly IIngredientService _ingredients;
    public ProductsApiController(IProductService products, IIngredientService ingredients)
    { _products = products; _ingredients = ingredients; }

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

// ── Ingredients ───────────────────────────────────────────────────────────────
[ApiController]
[Route("api/ingredients")]
[Authorize]
public class IngredientsApiController : ControllerBase
{
    private readonly IIngredientService _service;
    public IngredientsApiController(IIngredientService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateIngredientDto dto)
        => Ok(await _service.CreateAsync(dto));

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var ok = await _service.DeleteAsync(id);
        return ok ? Ok() : NotFound();
    }
}

// ── Routines ──────────────────────────────────────────────────────────────────
[ApiController]
[Route("api/routines")]
[Authorize]
public class RoutinesApiController : ControllerBase
{
    private readonly IRoutineService _routines;
    public RoutinesApiController(IRoutineService routines) { _routines = routines; }

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

// ── Skin Logs ─────────────────────────────────────────────────────────────────
[ApiController]
[Route("api/skinlogs")]
[Authorize]
public class SkinLogsApiController : ControllerBase
{
    private readonly ISkinLogService _service;
    public SkinLogsApiController(ISkinLogService service) => _service = service;

    private int GetUserId()
    {
        var uid = User.FindFirstValue("uid")
               ?? User.FindFirstValue(ClaimTypes.NameIdentifier)
               ?? "0";
        return int.TryParse(uid, out var id) ? id : 0;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(await _service.GetByUserAsync(GetUserId()));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSkinLogDto dto)
        => Ok(await _service.CreateAsync(GetUserId(), dto));

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateSkinLogDto dto)
    {
        var result = await _service.UpdateAsync(id, GetUserId(), dto);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var ok = await _service.DeleteAsync(id, GetUserId());
        return ok ? Ok() : NotFound();
    }

    [HttpGet("streak")]
    public async Task<IActionResult> Streak()
        => Ok(await _service.GetStreakAsync(GetUserId()));

    [HttpGet("progress")]
    public async Task<IActionResult> Progress([FromQuery] string? from, [FromQuery] string? to)
    {
        var fromDt = from != null ? DateTime.Parse(from).ToUniversalTime() : DateTime.UtcNow.AddDays(-29);
        var toDt = to != null ? DateTime.Parse(to).ToUniversalTime() : DateTime.UtcNow;
        return Ok(await _service.GetProgressAsync(GetUserId(), fromDt, toDt));
    }
}

// ── Users (Admin) ─────────────────────────────────────────────────────────────
[ApiController]
[Route("api/users")]
[Authorize(Roles = "Admin")]
public class UsersApiController : ControllerBase
{
    private readonly IUserRepository _users;
    public UsersApiController(IUserRepository users) => _users = users;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = (await _users.GetAllAsync())
            .Select(u => new UserDto(u.Id, u.Name, u.Email, u.Role, u.SkinType, u.SkinConcerns, u.CreatedAt));
        return Ok(users);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _users.DeleteAsync(id);
        return Ok();
    }
}