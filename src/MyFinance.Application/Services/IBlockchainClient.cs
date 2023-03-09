namespace MyFinance.Application.Services;

public interface IBlockchainClient
{
    Task<decimal> GetBalanceAsync(string address);
}