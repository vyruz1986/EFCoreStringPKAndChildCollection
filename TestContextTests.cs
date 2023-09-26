using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Testcontainers.MsSql;

namespace EFCoreStringPKAndChildCollection;

public class TestContextTests : IAsyncLifetime
{
    private DbContextOptions<TestContext> ContextOptions { get; set; } = default!;

    private readonly MsSqlContainer _dbContainer = new MsSqlBuilder().Build();

    public async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
    }
    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        ContextOptions = new DbContextOptionsBuilder<TestContext>()
            .UseSqlServer(_dbContainer.GetConnectionString())
            .LogTo(Console.WriteLine)
            .EnableSensitiveDataLogging()
            .Options;

        using var context = new TestContext(ContextOptions);
        await context.Database.EnsureCreatedAsync();
    }

    [Fact]
    public async void ShouldAddNewChildrenToCollectionForStringPK()
    {
        // Given
        var parentId = Guid.NewGuid().ToString();
        var generatedParent = new ParentWithStringPK
        {
            Id = parentId,
            Children = new() { new(Guid.NewGuid()) }
        };
        var childToAdd = new Child(Guid.NewGuid());

        using (var context = new TestContext(ContextOptions))
        {
            context.ParentsWithString.Add(generatedParent);
            await context.SaveChangesAsync();
        }

        // When
        using (var context = new TestContext(ContextOptions))
        {
            var parent = await context.ParentsWithString.SingleAsync(p => p.Id == parentId);
            parent.Children.Add(childToAdd);
            await context.SaveChangesAsync();
        }

        // Then
        using (var context = new TestContext(ContextOptions))
        {
            var parent = await context.ParentsWithString.SingleAsync(p => p.Id == parentId);
            parent.Children.Should().Contain(childToAdd);
        }
    }

    [Fact]
    public async void ShouldAddNewChildrenToCollectionForGuidPK()
    {
        // Given
        var parentId = Guid.NewGuid();
        var generatedParent = new ParentWithGuidPK
        {
            Id = parentId,
            Children = new() { new(Guid.NewGuid()) }
        };
        var childToAdd = new Child(Guid.NewGuid());

        using (var context = new TestContext(ContextOptions))
        {
            context.ParentsWithGuid.Add(generatedParent);
            await context.SaveChangesAsync();
        }

        // When
        using (var context = new TestContext(ContextOptions))
        {
            var parent = await context.ParentsWithGuid.SingleAsync(p => p.Id == parentId);
            parent.Children.Add(childToAdd);
            await context.SaveChangesAsync();
        }

        // Then
        using (var context = new TestContext(ContextOptions))
        {
            var parent = await context.ParentsWithGuid.SingleAsync(p => p.Id == parentId);
            parent.Children.Should().Contain(childToAdd);
        }
    }
}
