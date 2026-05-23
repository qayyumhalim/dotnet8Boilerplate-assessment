using AcmePortal.Data;
using AcmePortal.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AcmePortal.Pages.Customers;

[Authorize]
public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public IList<Customer> Customer { get; set; } = [];

    public async Task OnGetAsync()
    {
        Customer = await _context.Customers
            .Where(x => x.IsDelete == false)
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .ToListAsync();
    }
}
