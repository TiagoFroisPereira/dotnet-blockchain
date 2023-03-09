using System.Numerics;
using Nethereum.Web3;
using Nethereum.Contracts;
using Nethereum.ABI.FunctionEncoding.Attributes;
using MyFinance.Application.Services;
using MyFinance.Domain.Aggregates;
using Mapster;
using Microsoft.Extensions.Options;
using MyFinance.Infrastructure.Blockchain;

[FunctionOutput]
public class TransactionDto
{
    [Parameter("string", "description", 1)]
    public string Description { get; set; }

    [Parameter("uint256", "amount", 2)]
    public BigInteger Amount { get; set; }

    [Parameter("bool", "isExpense", 3)]
    public bool IsExpense { get; set; }

    [Parameter("bool", "isPaid", 4)]
    public bool IsPaid { get; set; }

    [Parameter("uint256", "dueDate", 5)]
    public BigInteger DueDate { get; set; }
}



public class FinanceContract : IFinanceContract
{
    private readonly Web3 _web3;
    private readonly Contract _contract;

    public FinanceContract(IOptions<BlockchainOptions> blockchainOptions)
    {
        string abiJson = File.ReadAllText(blockchainOptions.Value.Contracts.Finance.AbiPath);
        _web3 = new Web3(blockchainOptions.Value.RpcUrl);
        _contract = _web3.Eth.GetContract(abiJson, blockchainOptions.Value.Contracts.Finance.ContractAddress);
    }

    public async Task<BigInteger> GetTransactionCountAsync()
    {
        var function = _contract.GetFunction("getTransactionCount");
        var transactionCount = await function.CallAsync<BigInteger>();

        return transactionCount;
    }

    public async Task<MyTransaction> GetTransactionAsync(BigInteger index)
    {
        var function = _contract.GetFunction("getTransaction");

        var result = await function.CallDeserializingToObjectAsync<TransactionDto>(index);

        return result.Adapt<MyTransaction>();
    }

    public async Task AddTransactionAsync(string description, BigInteger amount, bool isExpense, BigInteger dueDate)
    {
        var function = _contract.GetFunction("addTransaction");

        TransactionInput transactionInput = new ()
        {
            Description = description,
            Amount = amount,
            IsExpense = isExpense,
            DueDate = dueDate
        };

        await function.SendTransactionAsync(_web3.TransactionManager.Account.Address, null, transactionInput);
    }

    public async Task UpdateTransactionAsync(BigInteger index, string description, BigInteger amount, bool isExpense, BigInteger dueDate)
    {
        var function = _contract.GetFunction("updateTransaction");

        TransactionInput transactionInput = new ()
        {
            Description = description,
            Amount = amount,
            IsExpense = isExpense,
            DueDate = dueDate
        };

        await function.SendTransactionAsync(_web3.TransactionManager.Account.Address, null, index, transactionInput);
    }

    public async Task SetTransactionPaidAsync(BigInteger index, bool isPaid)
    {
        var function = _contract.GetFunction("setTransactionPaid");

        await function.SendTransactionAsync(_web3.TransactionManager.Account.Address, null, index, isPaid);
    }

    public async Task SetTransactionDueDateAsync(BigInteger index, BigInteger dueDate)
    {
        var function = _contract.GetFunction("setTransactionDueDate");

        await function.SendTransactionAsync(_web3.TransactionManager.Account.Address, null, index, dueDate);
    }

    private class TransactionInput
    {
        [Parameter("string", "description", 1)]
        public string Description { get; set; }

        [Parameter("uint256", "amount", 2)]
        public BigInteger Amount { get; set; }

        [Parameter("bool", "isExpense", 3)]
        public bool IsExpense { get; set; }

        [Parameter("uint256", "dueDate", 4)]
        public BigInteger DueDate { get; set; }
    }
}
