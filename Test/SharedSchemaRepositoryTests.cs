using Microsoft.EntityFrameworkCore;
using Schematics.API.Data;
using Schematics.API.Data.Entities;
using Schematics.API.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Assert = Xunit.Assert;

public class SharedSchemaRepositoryTests
{
    private ApplicationDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase($"SharedSchemaDb_{Guid.NewGuid()}")
            .Options;

        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task AddAsync_AddsSharedSchema()
    {
        var context = GetInMemoryDbContext();
        var repo = new SharedSchemaRepository(context);

        var shared = new SharedSchemaDb
        {
            SchemaId = 1,
            OwnerId = "owner1",
            SharedWithUserId = "user1"
        };

        await repo.AddAsync(shared);
        await repo.SaveChangesAsync();

        var all = await context.SharedSchemas.ToListAsync();
        Assert.Single(all);
        Assert.Equal("owner1", all.First().OwnerId);
        Assert.Equal("user1", all.First().SharedWithUserId);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsSharedSchema_WhenExists()
    {
        var context = GetInMemoryDbContext();
        var shared = new SharedSchemaDb { SchemaId = 1, OwnerId = "owner1", SharedWithUserId = "user1" };
        await context.SharedSchemas.AddAsync(shared);
        await context.SaveChangesAsync();

        var repo = new SharedSchemaRepository(context);

        var result = await repo.GetByIdAsync(shared.Id);

        Assert.NotNull(result);
        Assert.Equal(shared.Id, result.Id);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotExists()
    {
        var context = GetInMemoryDbContext();
        var repo = new SharedSchemaRepository(context);

        var result = await repo.GetByIdAsync(999);
        Assert.Null(result);
    }

    [Fact]
    public async Task GetBySchemaAndUserAsync_ReturnsSharedSchema_WhenExists()
    {
        var context = GetInMemoryDbContext();
        var shared = new SharedSchemaDb { SchemaId = 1, OwnerId = "owner1", SharedWithUserId = "user1" };
        await context.SharedSchemas.AddAsync(shared);
        await context.SaveChangesAsync();

        var repo = new SharedSchemaRepository(context);

        var result = await repo.GetBySchemaAndUserAsync(1, "user1");

        Assert.NotNull(result);
        Assert.Equal(shared.Id, result.Id);
    }

    [Fact]
    public async Task GetBySchemaAndUserAsync_ReturnsNull_WhenNotExists()
    {
        var context = GetInMemoryDbContext();
        var repo = new SharedSchemaRepository(context);

        var result = await repo.GetBySchemaAndUserAsync(1, "user1");

        Assert.Null(result);
    }

    [Fact]
    public async Task GetSharedWithUserAsync_ReturnsOnlyMatchingUser()
    {
        var context = GetInMemoryDbContext();
        await context.SharedSchemas.AddRangeAsync(new List<SharedSchemaDb>
        {
            new SharedSchemaDb { SchemaId = 1, OwnerId = "owner1", SharedWithUserId = "user1" },
            new SharedSchemaDb { SchemaId = 2, OwnerId = "owner2", SharedWithUserId = "user2" }
        });
        await context.SaveChangesAsync();

        var repo = new SharedSchemaRepository(context);

        var result = await repo.GetSharedWithUserAsync("user1");

        Assert.Single(result);
        Assert.Equal("user1", result.First().SharedWithUserId);
    }

    [Fact]
    public async Task GetSharedByOwnerAsync_ReturnsOnlyMatchingOwner()
    {
        var context = GetInMemoryDbContext();
        await context.SharedSchemas.AddRangeAsync(new List<SharedSchemaDb>
        {
            new SharedSchemaDb { SchemaId = 1, OwnerId = "owner1", SharedWithUserId = "user1" },
            new SharedSchemaDb { SchemaId = 2, OwnerId = "owner2", SharedWithUserId = "user2" }
        });
        await context.SaveChangesAsync();

        var repo = new SharedSchemaRepository(context);

        var result = await repo.GetSharedByOwnerAsync("owner1");

        Assert.Single(result);
        Assert.Equal("owner1", result.First().OwnerId);
    }

    [Fact]
    public async Task Update_UpdatesSharedSchema()
    {
        var context = GetInMemoryDbContext();
        var shared = new SharedSchemaDb { SchemaId = 1, OwnerId = "owner1", SharedWithUserId = "user1" };
        await context.SharedSchemas.AddAsync(shared);
        await context.SaveChangesAsync();

        var repo = new SharedSchemaRepository(context);
        shared.SharedWithUserId = "user2";
        repo.Update(shared);
        await repo.SaveChangesAsync();

        var updated = await context.SharedSchemas.FindAsync(shared.Id);
        Assert.Equal("user2", updated.SharedWithUserId);
    }

    [Fact]
    public async Task Remove_DeletesSharedSchema()
    {
        var context = GetInMemoryDbContext();
        var shared = new SharedSchemaDb { SchemaId = 1, OwnerId = "owner1", SharedWithUserId = "user1" };
        await context.SharedSchemas.AddAsync(shared);
        await context.SaveChangesAsync();

        var repo = new SharedSchemaRepository(context);
        repo.Remove(shared);
        await repo.SaveChangesAsync();

        var deleted = await context.SharedSchemas.FindAsync(shared.Id);
        Assert.Null(deleted);
    }
}
