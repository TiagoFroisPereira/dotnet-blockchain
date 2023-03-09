using System.Numerics;

namespace MyFinance.Domain.Aggregates;

public class MyTransaction
{
    public string Description { get; set; }

    public BigInteger Amount { get; set; }

    public bool IsExpense { get; set; }

    public bool IsPaid { get; set; }

    public BigInteger DueDate { get; set; }
}