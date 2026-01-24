using Microsoft.AspNetCore.Identity;
using Moq;
using Schematics.API.Data.Entities;
using Schematics.API.DTOs.Users;
using Schematics.API.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Assert = Xunit.Assert;

public class UserServiceTests
{
    private readonly Mock<UserManager<User>> _mockUserManager;
    private readonly Mock<RoleManager<IdentityRole>> _mockRoleManager;
    private readonly UserService _service;

    public UserServiceTests()
    {
        _mockUserManager = MockUserManager();
        _mockRoleManager = MockRoleManager();
        _service = new UserService(_mockRoleManager.Object, _mockUserManager.Object);
    }

    private Mock<UserManager<User>> MockUserManager()
    {
        var store = new Mock<IUserStore<User>>();
        return new Mock<UserManager<User>>(store.Object, null, null, null, null, null, null, null, null);
    }

    private Mock<RoleManager<IdentityRole>> MockRoleManager()
    {
        var store = new Mock<IRoleStore<IdentityRole>>();
        return new Mock<RoleManager<IdentityRole>>(store.Object, null, null, null, null);
    }

    [Fact]
    public async Task GetUserById_ReturnsDto_WhenUserExists()
    {
        var user = new User { Id = "1", UserName = "TestUser", Email = "test@test.com" };
        _mockUserManager.Setup(um => um.FindByIdAsync("1")).ReturnsAsync(user);

        var result = await _service.GetUserById("1");

        Assert.NotNull(result);
        Assert.Equal("TestUser", result.Name);
        Assert.Equal("test@test.com", result.Email);
    }

    [Fact]
    public async Task GetUserById_ReturnsNull_WhenUserDoesNotExist()
    {
        _mockUserManager.Setup(um => um.FindByIdAsync("1")).ReturnsAsync((User)null);

        var result = await _service.GetUserById("1");

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateUser_ReturnsTrue_WhenUpdateSucceeds()
    {
        var user = new User { Id = "1" };
        _mockUserManager.Setup(um => um.FindByIdAsync("1")).ReturnsAsync(user);
        _mockUserManager.Setup(um => um.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);

        var dto = new EditUserDto { Id = "1", Name = "NewName", Email = "new@test.com", Firstname = "F", Lastname = "L" };

        var result = await _service.UpdateUser(dto);

        Assert.True(result);
        Assert.Equal("NewName", user.UserName);
        Assert.Equal("new@test.com", user.Email);
    }

    [Fact]
    public async Task LockUser_SetsLockoutEnd()
    {
        var user = new User { Id = "1" };
        _mockUserManager.Setup(um => um.FindByIdAsync("1")).ReturnsAsync(user);
        _mockUserManager.Setup(um => um.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);

        var result = await _service.LockUser("1");

        Assert.True(result);
        Assert.NotNull(user.LockoutEnd);
        Assert.True(user.LockoutEnd > DateTimeOffset.UtcNow);
    }

    [Fact]
    public async Task UnlockUser_ClearsLockoutEnd()
    {
        var user = new User { Id = "1", LockoutEnd = DateTimeOffset.UtcNow.AddYears(1) };
        _mockUserManager.Setup(um => um.FindByIdAsync("1")).ReturnsAsync(user);
        _mockUserManager.Setup(um => um.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);

        var result = await _service.UnlockUser("1");

        Assert.True(result);
        Assert.Null(user.LockoutEnd);
    }

    [Fact]
    public async Task AddUserToRole_CreatesRoleIfNotExists_AndAddsUser()
    {
        var user = new User { Id = "1" };
        _mockUserManager.Setup(um => um.FindByIdAsync("1")).ReturnsAsync(user);
        _mockRoleManager.Setup(rm => rm.RoleExistsAsync("Admin")).ReturnsAsync(false);
        _mockRoleManager.Setup(rm => rm.CreateAsync(It.IsAny<IdentityRole>())).ReturnsAsync(IdentityResult.Success);
        _mockUserManager.Setup(um => um.AddToRoleAsync(user, "Admin")).ReturnsAsync(IdentityResult.Success);

        var result = await _service.AddUserToRole("1", "Admin");

        Assert.True(result);
        _mockRoleManager.Verify(rm => rm.CreateAsync(It.Is<IdentityRole>(r => r.Name == "Admin")), Times.Once);
        _mockUserManager.Verify(um => um.AddToRoleAsync(user, "Admin"), Times.Once);
    }

    [Fact]
    public async Task RemoveUserFromRole_ReturnsTrue_WhenSucceeds()
    {
        var user = new User { Id = "1" };
        _mockUserManager.Setup(um => um.FindByIdAsync("1")).ReturnsAsync(user);
        _mockUserManager.Setup(um => um.RemoveFromRoleAsync(user, "Admin")).ReturnsAsync(IdentityResult.Success);

        var result = await _service.RemoveUserFromRole("1", "Admin");

        Assert.True(result);
        _mockUserManager.Verify(um => um.RemoveFromRoleAsync(user, "Admin"), Times.Once);
    }
}

