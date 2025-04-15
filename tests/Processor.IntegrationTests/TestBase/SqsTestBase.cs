using System.Security.Cryptography;
using Amazon.Runtime;
using Amazon.SQS;
using Amazon.SQS.Model;
using Xunit.Abstractions;

namespace Defra.TradeImportsProcessor.Processor.IntegrationTests.TestBase;

public class SqsTestBase(ITestOutputHelper output)
{
    private const string QueueUrl =
        "http://sqs.eu-west-2.127.0.0.1:4566/000000000000/trade_imports_inbound_customs_declarations.fifo";

    private readonly AmazonSQSClient _sqsClient = new(
        new BasicAWSCredentials("test", "test"),
        new AmazonSQSConfig { AuthenticationRegion = "eu-west-2", ServiceURL = "http://localhost:4566" }
    );

    protected Task<ReceiveMessageResponse> ReceiveMessage()
    {
        return _sqsClient.ReceiveMessageAsync(QueueUrl, CancellationToken.None);
    }

    protected Task<GetQueueAttributesResponse> GetQueueAttributes()
    {
        return _sqsClient.GetQueueAttributesAsync(
            new GetQueueAttributesRequest { AttributeNames = ["ApproximateNumberOfMessages"], QueueUrl = QueueUrl },
            CancellationToken.None
        );
    }

    protected async Task SendMessage(
        string messageGroupId,
        string body,
        Dictionary<string, MessageAttributeValue>? messageAttributes = null
    )
    {
        var request = new SendMessageRequest
        {
            MessageAttributes = messageAttributes,
            MessageBody = body,
            MessageDeduplicationId = RandomNumberGenerator.GetString("abcdefg", 20),
            MessageGroupId = messageGroupId,
            QueueUrl = QueueUrl,
        };
        var result = await _sqsClient.SendMessageAsync(request, CancellationToken.None);
        output.WriteLine("Sent {0} to {1}", result.MessageId, QueueUrl);
    }
}
