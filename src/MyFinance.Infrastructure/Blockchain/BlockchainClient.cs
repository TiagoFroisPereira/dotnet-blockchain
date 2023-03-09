using Microsoft.Extensions.Options;
using MyFinance.Application.Services;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;

namespace MyFinance.Infrastructure.Blockchain;

public class BlockchainClient : IBlockchainClient
{
    private readonly Web3 _web3;

    public BlockchainClient(IOptions<BlockchainOptions> blockchainOptions)
    {
        _web3 = new Web3(blockchainOptions.Value.RpcUrl);
    }

    public async Task<decimal> GetBalanceAsync(string address)
    {
        var balance = await _web3.Eth.GetBalance.SendRequestAsync(address);
        return Web3.Convert.FromWei(balance.Value);
    }

    public async Task<string> SendTransactionAsync(string toAddress, decimal amount)
    {
        var fromAddress = _web3.TransactionManager.Account.Address;
        var weiAmount = Web3.Convert.ToWei(amount);
        var gasPrice = await _web3.Eth.GasPrice.SendRequestAsync();
        var gas = await _web3.TransactionManager.EstimateGasAsync(new CallInput { To = toAddress, Value = new HexBigInteger(weiAmount) });
        TransactionInput input = new()
        {
            From = fromAddress,
            To = toAddress,
            Value = new HexBigInteger(weiAmount),
            Gas = gas,
            GasPrice = new HexBigInteger(gasPrice)
        };
        var tx = await _web3.TransactionManager.SendTransactionAsync(input);
        return tx;
    }
}