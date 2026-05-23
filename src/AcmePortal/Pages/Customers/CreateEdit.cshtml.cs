using AcmePortal.Common;
using AcmePortal.Data;
using AcmePortal.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AcmePortal.Pages.Customers;

[Authorize(Roles = AppRoles.Admin)]
public class CreateEditModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public CreateEditModel(ApplicationDbContext context) => _context = context;

    [BindProperty]
    public Customer Customer { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id.HasValue)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(m => m.Id == id);

            if (customer == null)
                return NotFound();

            Customer = customer;
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        bool isEdit = Customer.Id > 0;

        if (!isEdit)
        {
            Customer.CreatedAt = DateTime.UtcNow;
            Customer.UpdatedAt = DateTime.UtcNow;

            _context.Customers.Add(Customer);

            _context.AuditLogs.Add(new AuditLog
            {
                EntityName = "Customer",
                EntityId = "0",
                Action = AuditType.Created,
                ChangedBy = User.Identity!.Name!,
                ChangedAt = DateTime.UtcNow
            });
        }
        else
        {
            var existing = await _context.Customers.FindAsync(Customer.Id);

            if (existing == null)
                return NotFound();

            existing.Name = Customer.Name;
            existing.Email = Customer.Email;
            existing.Phone = Customer.Phone;
            existing.IsActive = Customer.IsActive;
            existing.UpdatedAt = DateTime.UtcNow;

            _context.AuditLogs.Add(new AuditLog
            {
                EntityName = "Customer",
                EntityId = existing.Id.ToString(),
                Action = AuditType.Updated,
                ChangedBy = User.Identity!.Name!,
                ChangedAt = DateTime.UtcNow
            });
        }

        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}
