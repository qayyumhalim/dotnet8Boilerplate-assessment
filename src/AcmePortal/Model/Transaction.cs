using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcmePortal.Model
{
    public class Transaction
    {
        public int Id { get; set; }

        public string ReferenceNo { get; set; } = string.Empty;

        public DateTime PurchaseDate { get; set; }

        [Required]
        public int CustomerId { get; set; }

        public Customer? Customer { get; set; } = null!;

        [Required]
        public int ProductId { get; set; }

        public Product? Product { get; set; } = null!;

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Range(0, double.MaxValue)]
        public decimal TotalPrice { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
