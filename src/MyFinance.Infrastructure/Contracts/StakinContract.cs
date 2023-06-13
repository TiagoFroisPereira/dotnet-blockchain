using Nethereum.Web3;
using Nethereum.Contracts;
using Microsoft.Extensions.Options;
using MyFinance.Infrastructure.Blockchain;
using System.Numerics;
using Nethereum.Util;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace MyFinance.Infrastructure.Contracts;

public interface IStakinContract
{
    Task<decimal> GetTotalStaked();
}

public class StakinContract : IStakinContract
{
    private readonly Web3 _web3;
    private readonly Contract _contract;

    private const string address = "0xDee32d7BE22A893472c0B9fC30bd0afC2d9Cff83";

    public StakinContract(IOptions<BlockchainOptions> blockchainOptions)
    {
        string abiJson = File.ReadAllText(blockchainOptions.Value.Contracts.Stakin.AbiPath);
        _web3 = new Web3(blockchainOptions.Value.RpcUrl);
        _contract = _web3.Eth.GetContract(abiJson, blockchainOptions.Value.Contracts.Stakin.ContractAddress);
    }

    public async Task<decimal> GetTotalStaked()
    {
        var function = _contract.GetFunction("getTotalStaked");
        var stakedBalance = await function.CallAsync<BigInteger>(address);
        var pendingRewards = await GetPendingRewards();

        await GetAllClaimedRewards();
        return UnitConversion.Convert.FromWei(stakedBalance);
    }

    public async Task<decimal> GetPendingRewards()
    {
        var function = _contract.GetFunction("getLiquidRewards");
        var pendingRewards = await function.CallAsync<BigInteger>(address);

        return UnitConversion.Convert.FromWei(pendingRewards);
    }

    public async Task GetAllClaimedRewards()
    {

        // Define the event filter
        var eventDeclarator = _contract.GetEvent<DelegatorClaimendRewardsEvent>();
        var filterForAddress = eventDeclarator.CreateFilterInput();
        var events = await eventDeclarator.GetAllChangesAsync(filterForAddress);


        var eventTransfer = _contract.GetEvent<TransferEvent>();
        var filterAllTransferEventsForAddress = eventTransfer.CreateFilterInput(null, new[] { address });

        // Retrieve the events
        var transferEvents = await eventTransfer.GetAllChangesAsync(filterAllTransferEventsForAddress);

        return;
    }
}

// Define the event structure
[Event("DelegatorRestaked")]
public class DelegatorClaimendRewardsEvent : IEventDTO
{
    [Parameter("uint256", "validatorId", 1, true)]
    public string validatorId { get; set; }

    [Parameter("address", "user", 2, true)]
    public string user { get; set; }

    [Parameter("uint256", "rewards", 3, false)]
    public BigInteger Rewards { get; set; }
}

[Event("Transfer")]
public class TransferEvent : IEventDTO
{
    [Parameter("address", "from", 1, true)]
    public string From { get; set; }

    [Parameter("address", "to", 2, true)]
    public string To { get; set; }

    [Parameter("uint256", "value", 3, false)]
    public BigInteger Value { get; set; }
}