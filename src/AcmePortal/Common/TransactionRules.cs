using AcmePortal.Model;

namespace AcmePortal.Common;

public static class TransactionRules
{
    public static (bool IsValid, string? ErrorMessage)
        ValidateQuantity(Product product, int requestedQty)
    {
        if (requestedQty <= 0)
            return (false, "Quantity must be greater than zero.");

        if (requestedQty > product.Quantity)
        {
            return (
                false,
                $"Requested quantity exceeds available stock ({product.Quantity})."
            );
        }

        return (true, null);
    }

    public static decimal CalculateTotalPrice(
        decimal price,
        int quantity)
    {
        return price * quantity;
    }
}