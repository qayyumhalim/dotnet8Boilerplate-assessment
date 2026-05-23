namespace AcmePortal.DTO;

public class TransactionDto
{
    public int Id { get; set; }

    public string ReferenceNo { get; set; } = string.Empty;

    public DateTime PurchaseDate { get; set; }

    public string Customer { get; set; } = string.Empty;

    public string Product { get; set; } = string.Empty;

    public int Quantity { get; set; }

    public decimal TotalPrice { get; set; }

    public DateTime CreatedAt { get; set; }
}