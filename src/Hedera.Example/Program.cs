using System;
using System.Text;
using System.Threading.Tasks;
using Hashgraph;
using Hedera.Example.Dtos;
using Hedera.Example.Services;

namespace Hedera;


class Program
{
    static Gateway gateway = new Gateway(new Uri("https://testnet.hedera.com:50002"), 0, 0, 5);

    public static string publicKey = "302a300506032b6570032100bfa7f5d894e595d4a12994c15aa78f3a82c1c9d16a55ac3de6d61446564c2d04";
    public static string privateKey = "302e020100300506032b657004220420d541b592535a10540cd2ae87d8e7a2e5c1a54dac1941a872b12a5dd9d62f0f9e";
    static async Task Main(string[] args)
    {
        try
        {
            var consensusService = new ConsensusService();
            consensusService.SubscribeToTopic(new Address(0, 0, 494235), ReadMessages);
            // var topic = await consensusService.CreateTopic(".NET Sdk Example");
            for (int i = 0; i < 200; i++)
            {
                await consensusService.SendMessage("494235", new ConsensusMessage { TopicId = "0.0.494235", Message = $".NET SDK Subscribe TEST2- {i}" });
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex.Message);
            Console.Error.WriteLine(ex.StackTrace);
        }
    }

    private static async Task<Client> GetBalance()
    {
        var client = new Client(ctx =>
        {
            ctx.Gateway = gateway;// new Gateway(gatewayUrl, 0, 0, gatewayAccountNo);
        });
        var account = new Address(0, 0, 4361201);
        var balance = await client.GetAccountBalanceAsync(account);
        Console.WriteLine($"Account Balance for {account.AccountNum} is {balance:#,#} tinybars.");
        return client;
    }

    public static void ReadMessages(TopicMessage message)
    {
        Console.WriteLine($"Received Message: {Encoding.ASCII.GetString(message.Messsage.ToArray())}");
    }

    private static async Task<Address> CreateTopic()
    {
        var payer = new Address(0, 0, 4361201);
        var signatory = new Signatory(Hex.ToBytes(privateKey));
        var endorsement = new Endorsement(Hex.ToBytes(publicKey));

        await using var client = new Client(ctx =>
        {
            ctx.Gateway = gateway;
            ctx.Payer = payer;
            ctx.Signatory = signatory;
        });
        var topicParams = new CreateTopicParams
        {
            Administrator = endorsement,
            Memo = "Hello, World!",
            RenewAccount = payer,
            Signatory = signatory
        };

        var result = await client.CreateTopicWithRecordAsync(topicParams);

        Console.WriteLine($"Created Topic: {result.Topic}.");
        return result.Topic;
    }


    private static async Task SendMessageNotTopicOwner(Address topic)
    {
        var payer = new Address(0, 0, 4361194);
        var signatory = new Signatory(Hex.ToBytes("3030020100300706052b8104000a04220420efcf0d9ed65d7aa52c1ce5436fe03f75ad0d49676e1953e99443b1f8021bcd6d"));
        var endorsement = new Endorsement(Hex.ToBytes("302d300706052b8104000a03220003c76c6659a165f09f17e747ed0953c4f4bde39708a329780fcba4ec772d41293f"));

        await using var client = new Client(ctx =>
        {
            ctx.Gateway = gateway;
            ctx.Payer = payer;
        });

        var balance = await client.SubmitMessageAsync(topic, Encoding.ASCII.GetBytes("My extended Message"), signatory);

        Console.WriteLine($"Sent Message to Topic: {topic} with status :{balance.Status}.");
    }



}