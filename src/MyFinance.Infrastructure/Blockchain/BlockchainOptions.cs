namespace MyFinance.Infrastructure.Blockchain;

public class BlockchainOptions
{
    public static string Position = "blockchain";
    public string RpcUrl { get; set; }
    public Contracts Contracts { get; set; }
}
public class Contracts{
    public BlockchainContractInfo Finance { get; set; }
}

public class BlockchainContractInfo
{
    public string AbiPath { get; set; }
    public string ContractAddress { get; set; }
}