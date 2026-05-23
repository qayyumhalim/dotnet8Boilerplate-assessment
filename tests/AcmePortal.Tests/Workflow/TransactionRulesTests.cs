using AcmePortal.Common;
using AcmePortal.Model;

namespace AcmePortal.Tests.Workflow;

public class TransactionRulesTests
{
    [Fact]
    public void ValidateQuantity_WhenStockIsEnough_ShouldReturnValid()
    {
        var product = new Product
        {
            Id = 1,
            Name = "Laptop",
            Quantity = 10,
            Price = 1000
        };

        int requestedQty = 5;

        var result = TransactionRules.ValidateQuantity(
            product,
            requestedQty);

        Assert.True(result.IsValid);
        Assert.Null(result.ErrorMessage);
    }

    [Fact]
    public void ValidateQuantity_WhenStockIsInsufficient_ShouldReturnInvalid()
    {
        var product = new Product
        {
            Id = 1,
            Name = "Laptop",
            Quantity = 3,
            Price = 1000
        };

        int requestedQty = 5;

        var result = TransactionRules.ValidateQuantity(
            product,
            requestedQty);

        Assert.False(result.IsValid);

        Assert.Equal(
            "Requested quantity exceeds available stock (3).",
            result.ErrorMessage);
    }

    [Fact]
    public void ValidateQuantity_WhenQuantityIsZero_ShouldReturnInvalid()
    {
        var product = new Product
        {
            Id = 1,
            Name = "Laptop",
            Quantity = 10,
            Price = 1000
        };

        int requestedQty = 0;

        var result = TransactionRules.ValidateQuantity(
            product,
            requestedQty);

        Assert.False(result.IsValid);

        Assert.Equal(
            "Quantity must be greater than zero.",
            result.ErrorMessage);
    }

    [Fact]
    public void CalculateTotalPrice_ShouldReturnCorrectValue()
    {
        decimal price = 25.50m;
        int quantity = 4;

        var total = TransactionRules.CalculateTotalPrice(
            price,
            quantity);

        Assert.Equal(102.00m, total);
    }
}