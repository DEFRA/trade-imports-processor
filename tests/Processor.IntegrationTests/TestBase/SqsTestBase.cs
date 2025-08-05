using System.Security.Cryptography;
using Amazon.Runtime;
using Amazon.SQS;
using Amazon.SQS.Model;
using Defra.TradeImportsProcessor.Processor.IntegrationTests.Helpers;
using Xunit.Abstractions;

namespace Defra.TradeImportsProcessor.Processor.IntegrationTests.TestBase;

public class SqsTestBase(ITestOutputHelper output) : TestBase
{
    private const string QueueUrl =
        "http://sqs.eu-west-2.127.0.0.1:4566/000000000000/trade_imports_inbound_customs_declarations_processor.fifo";

    private readonly AmazonSQSClient _sqsClient = new(
        new BasicAWSCredentials("test", "test"),
        new AmazonSQSConfig
        {
            AuthenticationRegion = "eu-west-2",
            ServiceURL = "http://localhost:4566",
            Timeout = TimeSpan.FromSeconds(5),
            MaxErrorRetry = 0,
        }
    );

    private Task<ReceiveMessageResponse> ReceiveMessage()
    {
        return _sqsClient.ReceiveMessageAsync(
            new ReceiveMessageRequest
            {
                QueueUrl = QueueUrl,
                MaxNumberOfMessages = 10,
                WaitTimeSeconds = 0,
            },
            CancellationToken.None
        );
    }

    protected Task<GetQueueAttributesResponse> GetQueueAttributes()
    {
        return _sqsClient.GetQueueAttributesAsync(
            new GetQueueAttributesRequest { AttributeNames = ["ApproximateNumberOfMessages"], QueueUrl = QueueUrl },
            CancellationToken.None
        );
    }

    protected async Task DrainAllMessages()
    {
        Assert.True(
            await AsyncWaiter.WaitForAsync(async () =>
            {
                var response = await ReceiveMessage();

                foreach (var message in response.Messages)
                {
                    output?.WriteLine("Drain message: {0} {1}", message.MessageId, message.Body);

                    await _sqsClient.DeleteMessageAsync(
                        new DeleteMessageRequest { QueueUrl = QueueUrl, ReceiptHandle = message.ReceiptHandle },
                        CancellationToken.None
                    );
                }

                var approximateNumberOfMessages = (await GetQueueAttributes()).ApproximateNumberOfMessages;

                output?.WriteLine("ApproximateNumberOfMessages: {0}", approximateNumberOfMessages);

                return approximateNumberOfMessages == 0;
            })
        );
    }

    protected async Task<string> SendMessage(
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

        return result.MessageId;
    }
}
