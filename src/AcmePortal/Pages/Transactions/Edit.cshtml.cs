using AcmePortal.Common;
using AcmePortal.Data;
using AcmePortal.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AcmePortal.Pages.TransactionPages;

[Authorize]
public class EditModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public EditModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Transaction Transaction { get; set; } = new();

    public List<Customer> Customers { get; set; } = [];
    public List<Product> Products { get; set; } = [];

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Transaction = await _context.Transactions
            .FirstOrDefaultAsync(x => x.Id == id);

        if (Transaction == null)
            return NotFound();

        Customers = await _context.Customers.ToListAsync();
        Products = await _context.Products.ToListAsync();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        Customers = await _context.Customers.ToListAsync();
        Products = await _context.Products.ToListAsync();

        if (!ModelState.IsValid)
            return Page();

        var existing = await _context.Transactions
            .FirstOrDefaultAsync(x => x.Id == Transaction.Id);

        if (existing == null)
            return NotFound();

        var product = await _context.Products
            .FirstOrDefaultAsync(x => x.Id == Transaction.ProductId);

        if (product == null)
            return NotFound();

        int oldQty = existing.Quantity;
        int newQty = Transaction.Quantity;

        int balanceQty = product.Quantity + oldQty;

        var validation = TransactionRules.ValidateQuantity(
            new Product { Quantity = balanceQty },
            newQty);

        if (!validation.IsValid)
        {
            ModelState.AddModelError(
                string.Empty,
                validation.ErrorMessage!);

            return Page();
        }

        product.Quantity = balanceQty - newQty;

        existing.CustomerId = Transaction.CustomerId;
        existing.ProductId = Transaction.ProductId;
        existing.Quantity = newQty;
        existing.TotalPrice =
            TransactionRules.CalculateTotalPrice(
                product.Price,
                newQty);

        existing.PurchaseDate = DateTime.UtcNow;

        _context.AuditLogs.Add(new AuditLog
        {
            EntityName = "Transaction",
            EntityId = existing.Id.ToString(),
            Action = AuditType.Updated,
            ChangedBy = User.Identity!.Name!,
            ChangedAt = DateTime.UtcNow,
            Remarks = $"Transaction updated: {existing.ReferenceNo}"
        });

        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}