using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using SkincareTracker.API.DTOs;
using SkincareTracker.API.Interfaces;
using SkincareTracker.API.Models;
using SkincareTracker.API.Services;
using Xunit;

namespace SkincareTracker.Tests;

// ── SkinLog Service Tests ─────────────────────────────────────────────────────
public class SkinLogServiceTests
{
    private readonly Mock<ISkinLogRepository> _repoMock = new();
    private SkinLogService CreateService() => new(_repoMock.Object);

    [Fact]
    public async Task GetStreak_NoLogs_ReturnsZeros()
    {
        _repoMock.Setup(r => r.GetByUserIdAsync(1))
            .ReturnsAsync(new List<SkinLog>());

        var result = await CreateService().GetStreakAsync(1);

        result.CurrentStreak.Should().Be(0);
        result.LongestStreak.Should().Be(0);
        result.TotalLogs.Should().Be(0);
    }

    [Fact]
    public async Task GetStreak_ConsecutiveDays_CorrectStreak()
    {
        var today = DateTime.UtcNow.Date;
        var logs = new List<SkinLog>
        {
            new() { UserId = 1, Date = today.AddDays(-2), SkinConditionScore = 7 },
            new() { UserId = 1, Date = today.AddDays(-1), SkinConditionScore = 8 },
            new() { UserId = 1, Date = today,             SkinConditionScore = 9 }
        };
        _repoMock.Setup(r => r.GetByUserIdAsync(1)).ReturnsAsync(logs);

        var result = await CreateService().GetStreakAsync(1);

        result.CurrentStreak.Should().Be(3);
        result.LongestStreak.Should().Be(3);
        result.TotalLogs.Should().Be(3);
    }

    [Fact]
    public async Task GetStreak_BrokenStreak_CurrentIsOne()
    {
        var today = DateTime.UtcNow.Date;
        var logs = new List<SkinLog>
        {
            new() { UserId = 1, Date = today.AddDays(-5), SkinConditionScore = 6 },
            new() { UserId = 1, Date = today.AddDays(-4), SkinConditionScore = 6 },
            new() { UserId = 1, Date = today,             SkinConditionScore = 8 }
        };
        _repoMock.Setup(r => r.GetByUserIdAsync(1)).ReturnsAsync(logs);

        var result = await CreateService().GetStreakAsync(1);

        result.CurrentStreak.Should().Be(1);
        result.LongestStreak.Should().Be(2);
    }

    [Fact]
    public async Task GetProgress_ReturnsCorrectRange()
    {
        var from = DateTime.UtcNow.Date.AddDays(-7);
        var to   = DateTime.UtcNow.Date;
        var logs = new List<SkinLog>
        {
            new() { UserId = 1, Date = from.AddDays(1), SkinConditionScore = 5, Hydration = "Medium", Oiliness = "High", Sensitivity = "Low" },
            new() { UserId = 1, Date = from.AddDays(3), SkinConditionScore = 7, Hydration = "High",   Oiliness = "Low",  Sensitivity = "Low" }
        };
        _repoMock.Setup(r => r.GetByUserIdAndRangeAsync(1, from, to)).ReturnsAsync(logs);

        var result = await CreateService().GetProgressAsync(1, from, to);

        result.Should().HaveCount(2);
        result[0].Score.Should().Be(5);
        result[1].Score.Should().Be(7);
    }

    [Fact]
    public async Task CreateLog_ReturnsCorrectDto()
    {
        var dto = new CreateSkinLogDto(DateTime.UtcNow, 8, "High", "Low", "Low", "Feeling good", "1,2");
        var saved = new SkinLog { Id = 42, UserId = 1, Date = dto.Date, SkinConditionScore = 8,
            Hydration = "High", Oiliness = "Low", Sensitivity = "Low", Notes = "Feeling good", ProductsUsed = "1,2" };

        _repoMock.Setup(r => r.CreateAsync(It.IsAny<SkinLog>())).ReturnsAsync(saved);

        var result = await CreateService().CreateAsync(1, dto);

        result.Id.Should().Be(42);
        result.SkinConditionScore.Should().Be(8);
        result.Hydration.Should().Be("High");
    }

    [Fact]
    public async Task DeleteLog_WrongUser_ReturnsFalse()
    {
        var log = new SkinLog { Id = 1, UserId = 99 };
        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(log);

        var result = await CreateService().DeleteAsync(1, userId: 1); // userId 1 ≠ owner 99

        result.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteLog_CorrectUser_ReturnsTrue()
    {
        var log = new SkinLog { Id = 1, UserId = 1 };
        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(log);
        _repoMock.Setup(r => r.DeleteAsync(1)).Returns(Task.CompletedTask);

        var result = await CreateService().DeleteAsync(1, userId: 1);

        result.Should().BeTrue();
    }
}

// ── Auth Service Tests ────────────────────────────────────────────────────────
public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _usersMock = new();
    private readonly Mock<IConfiguration> _configMock = new();

    private AuthService CreateService()
    {
        _configMock.Setup(c => c["Jwt:Key"]).Returns("SuperSecretSkincareTrackerKey2024!XYZ");
        return new AuthService(_usersMock.Object, _configMock.Object);
    }

    [Fact]
    public async Task Register_NewEmail_ReturnsTrue()
    {
        _usersMock.Setup(r => r.GetByEmailAsync("new@test.com")).ReturnsAsync((User?)null);
        _usersMock.Setup(r => r.CreateAsync(It.IsAny<User>())).ReturnsAsync(new User { Id = 1 });

        var result = await CreateService().RegisterAsync("Test", "new@test.com", "password123", "Oily", "Acne");

        result.Should().BeTrue();
    }

    [Fact]
    public async Task Register_ExistingEmail_ReturnsFalse()
    {
        _usersMock.Setup(r => r.GetByEmailAsync("existing@test.com"))
            .ReturnsAsync(new User { Email = "existing@test.com" });

        var result = await CreateService().RegisterAsync("Test", "existing@test.com", "password123", "Dry", "");

        result.Should().BeFalse();
    }

    [Fact]
    public async Task Login_WrongPassword_ReturnsNull()
    {
        var hash = BCrypt.Net.BCrypt.HashPassword("correct");
        _usersMock.Setup(r => r.GetByEmailAsync("u@test.com"))
            .ReturnsAsync(new User { Email = "u@test.com", PasswordHash = hash, Role = "User" });

        var token = await CreateService().AuthenticateAsync("u@test.com", "wrongpassword");

        token.Should().BeNull();
    }

    [Fact]
    public async Task Login_CorrectCredentials_ReturnsToken()
    {
        var hash = BCrypt.Net.BCrypt.HashPassword("correct");
        _usersMock.Setup(r => r.GetByEmailAsync("u@test.com"))
            .ReturnsAsync(new User { Id = 1, Email = "u@test.com", PasswordHash = hash, Role = "User" });

        var token = await CreateService().AuthenticateAsync("u@test.com", "correct");

        token.Should().NotBeNullOrEmpty();
        token.Should().Contain("."); // JWT format
    }
}

// ── Product Service Tests ─────────────────────────────────────────────────────
public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _repoMock = new();
    private ProductService CreateService() => new(_repoMock.Object);

    [Fact]
    public async Task GetById_NonExistent_ReturnsNull()
    {
        _repoMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Product?)null);

        var result = await CreateService().GetByIdAsync(999);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetById_Exists_ReturnsDto()
    {
        var product = new Product
        {
            Id = 1, Name = "CeraVe Moisturizer", Brand = "CeraVe",
            Category = "Moisturizer", Description = "Gentle moisturizer",
            ProductIngredients = new List<ProductIngredient>()
        };
        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);

        var result = await CreateService().GetByIdAsync(1);

        result.Should().NotBeNull();
        result!.Name.Should().Be("CeraVe Moisturizer");
        result.Brand.Should().Be("CeraVe");
    }

    [Fact]
    public async Task Delete_NonExistent_ReturnsFalse()
    {
        _repoMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Product?)null);

        var result = await CreateService().DeleteAsync(999);

        result.Should().BeFalse();
    }
}
