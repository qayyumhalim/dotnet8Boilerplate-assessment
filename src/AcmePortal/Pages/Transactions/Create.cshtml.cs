using AcmePortal.Common;
using AcmePortal.Data;
using AcmePortal.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AcmePortal.Pages.Transactions;

[Authorize]
public class CreateModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public CreateModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Transaction Transaction { get; set; } = new();

    public List<Customer> Customers { get; set; } = [];
    public List<Product> Products { get; set; } = [];

    public async Task OnGetAsync()
    {
        Customers = await _context.Customers
            .Where(x => !x.IsDelete)
            .ToListAsync();

        Products = await _context.Products
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        Customers = await _context.Customers.ToListAsync();
        Products = await _context.Products.ToListAsync();

        if (!ModelState.IsValid)
            return Page();

        var product = await _context.Products
            .FirstOrDefaultAsync(x => x.Id == Transaction.ProductId);

        if (product == null)
            return NotFound();

        var validation = TransactionRules.ValidateQuantity(
            product,
            Transaction.Quantity);

        if (!validation.IsValid)
        {
            ModelState.AddModelError(
                string.Empty,
                validation.ErrorMessage!);

            return Page();
        }

        Transaction.ReferenceNo =
            $"TRX-{DateTime.UtcNow:yyyyMMddHHmmss}";

        Transaction.PurchaseDate = DateTime.UtcNow;

        Transaction.TotalPrice =
            TransactionRules.CalculateTotalPrice(
                product.Price,
                Transaction.Quantity);

        Transaction.CreatedAt = DateTime.UtcNow;

        product.Quantity -= Transaction.Quantity;

        _context.Transactions.Add(Transaction);

        _context.AuditLogs.Add(new AuditLog
        {
            EntityName = "Transaction",
            EntityId = "0",
            Action = AuditType.Created,
            ChangedBy = User.Identity!.Name!,
            ChangedAt = DateTime.UtcNow,
            Remarks =
                $"Transaction created: {Transaction.ReferenceNo}"
        });

        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}
