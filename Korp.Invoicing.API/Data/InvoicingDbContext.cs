using Microsoft.EntityFrameworkCore;
using Korp.Invoicing.API.Models;

namespace Korp.Invoicing.API.Data;

public class InvoicingDbContext : DbContext
{
    public InvoicingDbContext(DbContextOptions<InvoicingDbContext> options) : base(options) { }

    public DbSet<Invoice> Invoices { get; set; }
    public DbSet<InvoiceItem> InvoiceItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Invoice>()
            .HasMany(i => i.Items)
            .WithOne(i => i.Invoice)
            .HasForeignKey(i => i.InvoiceId);
    }
}
