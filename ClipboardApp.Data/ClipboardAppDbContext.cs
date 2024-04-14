
using ClipboardApp.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace ClipboardApp.Data;

public class ClipboardAppDbContext : DbContext
{
    public ClipboardAppDbContext(DbContextOptions<ClipboardAppDbContext> options) : base(options)
    {
    }
    
    public DbSet<ClipboardText> ClipboardTexts { get; set; }
    public DbSet<BinaryItem> BinaryItems { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        base.OnModelCreating(modelBuilder);
    }
}