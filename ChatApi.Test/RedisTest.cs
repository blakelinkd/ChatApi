using Xunit;
using ChatApi;
using StackExchange.Redis;
using System;
using Moq;
using Microsoft.Extensions.Logging;
using ChatApi.Controllers;
using System.Text.Json;

namespace ChatApi.Tests.Controllers
{

    public class RedisTest
    {
        Message testMessage = new Message
        {
            MessageId = "msg123",
            ThreadId = "thread456",
            SenderId = "user789",
            RecipientId = "user101",
            Timestamp = DateTime.UtcNow,
            Content = new MessageContent
            {
                Text = "Hello, how are you?",
                Attachments = new List<Attachment>
        {
            new Attachment { Type = "image/jpeg", Url = "https://example.com/image.jpg" }
        }
            },
            Status = "sent",
            ResponseTo = null // or a valid message ID if it's a response
        };

        [Fact]
        public void CanConnectToRedis()
        {
            // Arrange
            var redisConnectionString = "localhost:6379"; // Update this with your Redis connection string
            var redis = ConnectionMultiplexer.Connect(redisConnectionString);
            var db = redis.GetDatabase();

            // Act & Assert
            Assert.True(redis.IsConnected);

            // Perform a simple test operation
            var key = "test_key";
            var value = "test_value";
            db.StringSet(key, value);
            var retrievedValue = db.StringGet(key);

            Assert.Equal(value, retrievedValue);
        }

        [Fact]
        public void Post_Message_ShouldEnqueueInRedis()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<ChatbotController>>();
            var mockRedisDb = new Mock<IDatabase>();
            var mockRedisMultiplexer = new Mock<IConnectionMultiplexer>();

            // Use a callback instead of directly setting up the return value
            mockRedisMultiplexer.Setup(_ => _.GetDatabase(It.IsAny<int>(), null))
                                .Returns(() => mockRedisDb.Object);

            var controller = new ChatbotController(mockLogger.Object, mockRedisMultiplexer.Object);
            var serializedMessage = JsonSerializer.Serialize(testMessage);

            // Act
            var result = controller.Post(testMessage);

            // Assert
            // Verify that the ListLeftPush method on the Redis database was called with the expected parameters
            mockRedisDb.Verify(db => db.ListLeftPush("messageQueue", serializedMessage, When.Always, CommandFlags.None), Times.Once);

            // Assert.IsType<SomeResponseType>(result);
        }
    }
}

