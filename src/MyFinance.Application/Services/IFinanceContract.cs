using System.Numerics;
using MyFinance.Domain.Aggregates;

namespace MyFinance.Application.Services;

public interface IFinanceContract
{
    Task AddTransactionAsync(string description, BigInteger amount, bool isExpense, BigInteger dueDate);
    Task<MyTransaction> GetTransactionAsync(BigInteger index);
    Task<BigInteger> GetTransactionCountAsync();
    Task SetTransactionDueDateAsync(BigInteger index, BigInteger dueDate);
    Task SetTransactionPaidAsync(BigInteger index, bool isPaid);
    Task UpdateTransactionAsync(BigInteger index, string description, BigInteger amount, bool isExpense, BigInteger dueDate);
}