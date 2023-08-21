using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Hashgraph;
using Hedera.Example.Dtos;

namespace Hedera.Example.Services;

public class ConsensusService : BaseService
{
    private readonly string accountId = "137033";
    private readonly string privateKey = "302e020100300506032b657004220420d541b592535a10540cd2ae87d8e7a2e5c1a54dac1941a872b12a5dd9d62f0f9e";
    private readonly string publicKey = "302a300506032b6570032100bfa7f5d894e595d4a12994c15aa78f3a82c1c9d16a55ac3de6d61446564c2d04";
    private Signatory _signatory;
    private Endorsement _endorsement;
    private readonly Address _account;
    public ConsensusService()
    {
        _account = new Address(0, 0, long.Parse(accountId));
        _signatory = new Signatory(Hex.ToBytes(privateKey));
        _endorsement = new Endorsement(Hex.ToBytes(publicKey));
    }

    public async Task<Address> CreateTopic(string topicName)
    {
        var gateway = await PickGatewayAsync();

        var client = new Client(ctx =>
        {
            ctx.Gateway = gateway;
            ctx.Payer = _account;
            ctx.Signatory = _signatory;
        });
        var topicParams = new CreateTopicParams
        {
            Administrator = _endorsement,
            Memo = topicName,
            RenewAccount = _account,
            Signatory = _signatory,

        };

        var result = await client.CreateTopicWithRecordAsync(topicParams);

        var topic = result.Topic;
        Console.WriteLine($"Created Topic: {topic}.");
        return topic;
    }

    public async Task<bool> SendMessage(string topicId, ConsensusMessage message)
    {
        var gateway = await PickGatewayAsync();
        var topic = new Address(0, 0, long.Parse(topicId));

        try
        {
            await using var client = new Client(ctx =>
            {
                ctx.Gateway = gateway;
                ctx.Payer = _account;
                ctx.Signatory = _signatory;
            });

            var balance = await client.SubmitMessageAsync(
                topic,
                Encoding.ASCII.GetBytes(JsonSerializer.Serialize(message)),
                _signatory);

            Console.WriteLine($"Sent Message to Topic: {topic} with status :{balance.Status}.");
        }
        catch (TransactionException e)
        {
            if (e.Status == ResponseCode.InvalidTopicId)
            {
                return false;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
        return true;
    }

    public async Task SubscribeToTopic(Address topic, Action<TopicMessage> readMessages)
    {
        var payer = new Address(0, 0, long.Parse(accountId));
        var signatory = new Signatory(Hex.ToBytes(privateKey));
        var endorsement = new Endorsement(Hex.ToBytes(publicKey));

        await using var client = new Client(ctx =>
        {
            ctx.Gateway = Gateway;
            ctx.Payer = payer;
        });
        var mirror = new MirrorGrpcClient(ctx =>
        {
            ctx.Uri = new Uri(MirrorNodeUrl);
        });

        await mirror.SubscribeTopicAsync(new SubscribeTopicParams
        {
            Topic = topic,
            Starting = DateTime.UtcNow.AddDays(-1),
            CompleteChannelWhenFinished = false,
            MessageWriter = new TopicMessageWriterAdapter(readMessages)
        });

    }

}