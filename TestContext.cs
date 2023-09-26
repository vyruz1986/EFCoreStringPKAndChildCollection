using Microsoft.EntityFrameworkCore;

namespace EFCoreStringPKAndChildCollection;

public class TestContext : DbContext
{
    public TestContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<ParentWithStringPK> ParentsWithString { get; set; }

    public DbSet<ParentWithGuidPK> ParentsWithGuid { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ParentWithStringPK>()
            .OwnsMany(e => e.Children);

        modelBuilder.Entity<ParentWithGuidPK>()
            .OwnsMany(e => e.Children);
    }
}