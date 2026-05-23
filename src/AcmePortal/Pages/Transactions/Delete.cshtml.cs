using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using AcmePortal.Model;
using AcmePortal.Data;

namespace AcmePortal.Pages.TransactionPages;

public class DeleteModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public DeleteModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Transaction Transaction { get; set; } = new();

    public List<Customer> Customers { get; set; } = [];
    public List<Product> Products { get; set; } = [];


    public async Task<IActionResult> OnGetAsync(int? id)
    {
        Transaction = await _context.Transactions
            .FirstOrDefaultAsync(x => x.Id == id);

        if (Transaction == null)
            return NotFound();

        Customers = await _context.Customers.ToListAsync();
        Products = await _context.Products.ToListAsync();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var transaction = await _context.Transactions.FindAsync(id);
        if (transaction != null)
        {
            Transaction = transaction;
            _context.Transactions.Remove(Transaction);
            await _context.SaveChangesAsync();
        }

        return RedirectToPage("./Index");
    }
}
