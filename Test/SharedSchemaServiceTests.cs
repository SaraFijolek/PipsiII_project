using Microsoft.EntityFrameworkCore;
using Moq;
using Schematics.API.Data;
using Schematics.API.Data.Entities;
using Schematics.API.Data.Repositories;
using Schematics.API.DTOs.SharedSchema;
using Schematics.API.Service;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Assert = Xunit.Assert;

public class SharedSchemaServiceTests
{
    private ApplicationDbContext GetInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: $"TestDb_{System.Guid.NewGuid()}")
            .Options;
        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task ShareSchemaAsync_AddsNewShare_WhenNotExists()
    {
        var context = GetInMemoryContext();
        var ownerId = "owner1";
        context.Schamas.Add(new SchamaDb { Id = 1, OwnerId = ownerId, Name = "A", City = "Abc", Country = "PL", Description = "" });
        await context.SaveChangesAsync();

        var mockRepo = new Mock<ISharedSchemaRepository>();
        mockRepo.Setup(r => r.GetBySchemaAndUserAsync(1, "user2")).ReturnsAsync((SharedSchemaDb)null);

        var service = new SharedSchemaService(mockRepo.Object, context);

        var dto = new ShareSchemaRequestDto
        {
            SchemaId = 1,
            SharedWithUserId = "user2",
            AccessLevel = "Read"
        };

        var result = await service.ShareSchemaAsync(ownerId, dto);

        Assert.True(result);
        mockRepo.Verify(r => r.AddAsync(It.Is<SharedSchemaDb>(
            s => s.SchemaId == 1 && s.OwnerId == ownerId && s.SharedWithUserId == "user2" && s.AccessLevel == "Read"
        )), Times.Once);
        mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task ShareSchemaAsync_UpdatesExistingShare_WhenExists()
    {
        var context = GetInMemoryContext();
        var ownerId = "owner1";
        context.Schamas.Add(new SchamaDb { Id = 1, OwnerId = ownerId, Name = "A", City = "Abc", Country = "PL", Description = "" });
        await context.SaveChangesAsync();

        var existing = new SharedSchemaDb { SchemaId = 1, OwnerId = ownerId, SharedWithUserId = "user2", AccessLevel = "Read" };
        var mockRepo = new Mock<ISharedSchemaRepository>();
        mockRepo.Setup(r => r.GetBySchemaAndUserAsync(1, "user2")).ReturnsAsync(existing);

        var service = new SharedSchemaService(mockRepo.Object, context);

        var dto = new ShareSchemaRequestDto
        {
            SchemaId = 1,
            SharedWithUserId = "user2",
            AccessLevel = "Write"
        };

        var result = await service.ShareSchemaAsync(ownerId, dto);

        Assert.True(result);
        Assert.Equal("Write", existing.AccessLevel);
        mockRepo.Verify(r => r.Update(existing), Times.Once);
        mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task ShareSchemaAsync_ReturnsFalse_WhenSchemaNotOwned()
    {
        var context = GetInMemoryContext();
        context.Schamas.Add(new SchamaDb { Id = 1, OwnerId = "otherOwner", Name = "A", City = "Abc", Country = "PL", Description = "" });
        await context.SaveChangesAsync();

        var mockRepo = new Mock<ISharedSchemaRepository>();
        var service = new SharedSchemaService(mockRepo.Object, context);

        var dto = new ShareSchemaRequestDto { SchemaId = 1, SharedWithUserId = "user2", AccessLevel = "Read" };
        var result = await service.ShareSchemaAsync("owner1", dto);

        Assert.False(result);
        mockRepo.Verify(r => r.AddAsync(It.IsAny<SharedSchemaDb>()), Times.Never);
    }

    [Fact]
    public async Task GetSharedWithUserAsync_ReturnsMappedDtos()
    {
        var mockRepo = new Mock<ISharedSchemaRepository>();
        mockRepo.Setup(r => r.GetSharedWithUserAsync("user2")).ReturnsAsync(new List<SharedSchemaDb>
        {
            new SharedSchemaDb
            {
                Id = 1,
                SchemaId = 10,
                SharedWithUserId = "user2",
                SharedWithUser = new User { Email = "user2@test.com" },
                AccessLevel = "Read"
            }
        });

        var service = new SharedSchemaService(mockRepo.Object, null);

        var result = await service.GetSharedWithUserAsync("user2");

        Assert.Single(result);
        Assert.Equal("user2@test.com", result[0].SharedWithEmail);
        Assert.Equal("Read", result[0].AccessLevel);
    }

    [Fact]
    public async Task GetSharedByOwnerAsync_ReturnsMappedDtos()
    {
        var mockRepo = new Mock<ISharedSchemaRepository>();
        mockRepo.Setup(r => r.GetSharedByOwnerAsync("owner1")).ReturnsAsync(new List<SharedSchemaDb>
        {
            new SharedSchemaDb
            {
                Id = 1,
                SchemaId = 10,
                OwnerId = "owner1",
                SharedWithUserId = "user2",
                SharedWithUser = new User { Email = "user2@test.com" },
                AccessLevel = "Write"
            }
        });

        var service = new SharedSchemaService(mockRepo.Object, null);

        var result = await service.GetSharedByOwnerAsync("owner1");

        Assert.Single(result);
        Assert.Equal("user2@test.com", result[0].SharedWithEmail);
        Assert.Equal("Write", result[0].AccessLevel);
    }

    [Fact]
    public async Task RemoveShareAsync_RemovesShare_WhenOwnerMatches()
    {
        var existing = new SharedSchemaDb { Id = 1, OwnerId = "owner1" };
        var mockRepo = new Mock<ISharedSchemaRepository>();
        mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);

        var service = new SharedSchemaService(mockRepo.Object, null);

        var result = await service.RemoveShareAsync(1, "owner1");

        Assert.True(result);
        mockRepo.Verify(r => r.Remove(existing), Times.Once);
        mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task RemoveShareAsync_ReturnsFalse_WhenOwnerMismatchOrNotFound()
    {
        var mockRepo = new Mock<ISharedSchemaRepository>();
        mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((SharedSchemaDb)null);

        var service = new SharedSchemaService(mockRepo.Object, null);

        var result = await service.RemoveShareAsync(1, "owner1");
        Assert.False(result);

        var existing = new SharedSchemaDb { Id = 2, OwnerId = "otherOwner" };
        mockRepo.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(existing);

        result = await service.RemoveShareAsync(2, "owner1");
        Assert.False(result);
    }
}

