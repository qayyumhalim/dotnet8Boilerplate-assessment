using AcmePortal.Data;
using AcmePortal.DTO;
using AcmePortal.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AcmePortal.Pages.TransactionPages;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }
    public IList<TransactionDto> Transaction { get; set; } = new List<TransactionDto>();

    public async Task OnGetAsync()
    {
        Transaction = await _context.Transactions
            .Include(x => x.Customer)
            .Include(x => x.Product)
            .Select(x => new TransactionDto
            {
                Id = x.Id,
                ReferenceNo = x.ReferenceNo,
                PurchaseDate = x.PurchaseDate,

                Customer = x.Customer != null
                    ? x.Customer.Name
                    : string.Empty,

                Product = x.Product != null
                    ? x.Product.Name
                    : string.Empty,

                Quantity = x.Quantity,
                TotalPrice = x.TotalPrice,
                CreatedAt = x.CreatedAt
            })
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }
}
