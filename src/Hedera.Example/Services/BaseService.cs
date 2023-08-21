using System;
using System.Linq;
using System.Threading.Tasks;
using Hashgraph;
using Hashgraph.Extensions;

namespace Hedera.Example.Services;

public class BaseService
{
    public Gateway Gateway { get; private set; }
    public readonly string gatewayUrl = "testnet" switch
    {
        "testnet" => "https://testnet.hedera.com:50002",
        "previewnet" => "previewnet.hedera.com:50211",
        _ => "mainnet.hedera.com:50211"
    };

    public readonly string MirrorNodeUrl = "testnet" switch
    {
        "testnet" => "https://testnet.mirrornode.hedera.com/",
        "previewnet" => "https://previewnet.mirrornode.hedera.com/",
        _ => "https://mainnet.mirrornode.hedera.com/"
    };

    /// <summary>
    ///   Gets the gateway.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<Gateway> PickGatewayAsync()
    {
        try
        {
            if (Gateway != null)
            {
                return Gateway;
            }
            var _mirrorClient = new MirrorRestClient(MirrorNodeUrl);
            var list = (await _mirrorClient.GetActiveGatewaysAsync(2000)).Keys.ToArray();
            if (list.Length == 0)
            {
                throw new Exception("Unable to find a target gossip node, no gossip endpoints are responding.");
            }
            Gateway = list[new Random().Next(0, list.Length)];
            return Gateway;
        }
        catch (Exception ex)
        {
            throw new Exception("Unable to find a target gossip node.", ex);
        }
    }

}