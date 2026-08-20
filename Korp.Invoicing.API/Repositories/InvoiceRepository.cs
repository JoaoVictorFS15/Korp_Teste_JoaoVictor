using Microsoft.EntityFrameworkCore;
using Korp.Invoicing.API.Data;
using Korp.Invoicing.API.Models;

namespace Korp.Invoicing.API.Repositories;

public interface IInvoiceRepository
{
    Task<IEnumerable<Invoice>> GetAllAsync();
    Task<Invoice?> GetByIdAsync(int id);
    Task<int> GetNextSequentialNumberAsync();
    Task AddAsync(Invoice invoice);
    Task UpdateAsync(Invoice invoice);
    Task SaveChangesAsync();
}

public class InvoiceRepository : IInvoiceRepository
{
    private readonly InvoicingDbContext _context;

    public InvoiceRepository(InvoicingDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Invoice>> GetAllAsync()
    {
        return await _context.Invoices
            .Include(i => i.Items)
            .ToListAsync();
    }

    public async Task<Invoice?> GetByIdAsync(int id)
    {
        return await _context.Invoices
            .Include(i => i.Items)
            .FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task<int> GetNextSequentialNumberAsync()
    {
        var lastInvoice = await _context.Invoices
            .OrderByDescending(i => i.SequentialNumber)
            .FirstOrDefaultAsync();
            
        return (lastInvoice?.SequentialNumber ?? 0) + 1;
    }

    public async Task AddAsync(Invoice invoice)
    {
        await _context.Invoices.AddAsync(invoice);
    }

    public Task UpdateAsync(Invoice invoice)
    {
        _context.Invoices.Update(invoice);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
