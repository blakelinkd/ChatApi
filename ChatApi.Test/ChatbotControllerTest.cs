using System;
using Microsoft.Extensions.Logging;
using Xunit;
using Moq;
using ChatApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System.Text.Json;
using ChatApi.Models;

namespace ChatApi.Tests.Controllers
{
    public class ChatbotControllerTests
    {
        private readonly Mock<ILogger<ChatbotController>> loggerMock;
        private readonly Mock<IConnectionMultiplexer> redisMultiplexerMock;
        private readonly Mock<IDatabase> redisDbMock;
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

        public ChatbotControllerTests()
        {
            loggerMock = new Mock<ILogger<ChatbotController>>();
            redisMultiplexerMock = new Mock<IConnectionMultiplexer>();
            redisDbMock = new Mock<IDatabase>();

            // Set up the mock RedisMultiplexer to return the mock Redis database
            redisMultiplexerMock.Setup(multiplexer => multiplexer.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
                                .Returns(redisDbMock.Object);
        }

        [Fact]
        public void Post_ReturnsOk()
        {
            // Arrange
            var controller = new ChatbotController(loggerMock.Object, redisMultiplexerMock.Object);
            var serializedMessage = JsonSerializer.Serialize(testMessage);

            // Act
            var result = controller.Post(testMessage);

            // Assert
            Assert.IsType<OkResult>(result);
            redisDbMock.Verify(db => db.ListLeftPush("messageQueue", serializedMessage, When.Always, CommandFlags.None), Times.Once);
        }

        [Fact]
        public void Post_ReturnsBadRequest()
        {
            // Arrange
            var controller = new ChatbotController(loggerMock.Object, redisMultiplexerMock.Object);

            // Act
            var result = controller.Post(message: null);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public void Post_Message_ShouldEnqueueInRedis()
        {
            // Arrange
            var controller = new ChatbotController(loggerMock.Object, redisMultiplexerMock.Object);
            var serializedMessage = JsonSerializer.Serialize(testMessage);

            // Act
            var result = controller.Post(testMessage);

            // Assert
            Assert.IsType<OkResult>(result);
            redisDbMock.Verify(db => db.ListLeftPush("messageQueue", serializedMessage, When.Always, CommandFlags.None), Times.Once);
        }

        [Fact]
        public void GetMessageQueue_ReturnsAllMessages()
        {
            // Arrange
            var controller = new ChatbotController(loggerMock.Object, redisMultiplexerMock.Object);

            // Create test messages
            var testMessages = new List<Message>
    {
        new Message { Content = new MessageContent { Text = "Message 1" } },
        new Message { Content = new MessageContent { Text = "Message 2" } },
        new Message { Content = new MessageContent { Text = "Message 3" } }
    };

            // Serialize test messages to JSON strings
            var serializedMessages = testMessages.Select(message => JsonSerializer.Serialize(message));

            // Mock the Redis database to simulate messages in the queue
            redisDbMock.Setup(db => db.ListRange("messageQueue", 0, -1, CommandFlags.None))
                       .Returns(() => serializedMessages.Select(message => (RedisValue)message).ToArray());

            // Act
            var actionResult = controller.GetMessageQueue();

            // Assert
            Assert.NotNull(actionResult);
            var okResult = actionResult.Result as OkObjectResult;
            Assert.NotNull(okResult);
            var resultValue = Assert.IsType<List<Message>>(okResult.Value);
            Assert.Equal(3, resultValue.Count);

            // Verify message content
            Assert.Equal("Message 1", resultValue[0].Content.Text);
            Assert.Equal("Message 2", resultValue[1].Content.Text);
            Assert.Equal("Message 3", resultValue[2].Content.Text);

            // Additional assertions as needed
        }

        [Fact]
        public void GetMessagesBySenderId_ReturnsCorrectMessages()
        {
            // Arrange
            var controller = new ChatbotController(loggerMock.Object, redisMultiplexerMock.Object);
            var senderId = "123";

            // Create test messages
            var testMessages = new List<Message>
    {
        new Message { SenderId = "123", Content = new MessageContent { Text = "Message 1" } },
        new Message { SenderId = "456", Content = new MessageContent { Text = "Message 2" } },
        new Message { SenderId = "123", Content = new MessageContent { Text = "Message 3" } }
    };

            // Serialize test messages to JSON strings
            var serializedMessages = testMessages.Select(message => JsonSerializer.Serialize(message));

            // Mock the Redis database to simulate messages in the queue
            redisDbMock.Setup(db => db.ListRange("messageQueue", 0, -1, CommandFlags.None))
                       .Returns(() => serializedMessages.Select(message => (RedisValue)message).ToArray());

            // Act
            var actionResult = controller.GetMessagesBySenderId(senderId);

            // Assert
            Assert.NotNull(actionResult);
            Assert.Equal(2, actionResult.Value.Count);

            // Verify message content
            Assert.Equal("Message 1", actionResult.Value[0].Content.Text);
            Assert.Equal("Message 3", actionResult.Value[1].Content.Text);

            // Verify senderId
            Assert.All(actionResult.Value, msg => Assert.Equal(senderId, msg.SenderId));
        }

        [Fact]
        public void GetMessagesByThreadId_ReturnsCorrectMessages()
        {
            // Arrange
            var controller = new ChatbotController(loggerMock.Object, redisMultiplexerMock.Object);
            var threadId = "abc123";

            // Create test messages
            var testMessages = new List<Message>
            {
                new Message { ThreadId = "abc123", Content = new MessageContent { Text = "Message 1" } },
                new Message { ThreadId = "def456", Content = new MessageContent { Text = "Message 2" } },
                new Message { ThreadId = "abc123", Content = new MessageContent { Text = "Message 3" } }
            };

            // Serialize test messages to JSON strings
            var serializedMessages = testMessages.Select(message => JsonSerializer.Serialize(message));

            // Mock the Redis database to simulate messages in the queue
            redisDbMock.Setup(db => db.ListRange("messageQueue", 0, -1, CommandFlags.None))
                       .Returns(() => serializedMessages.Select(message => (RedisValue)message).ToArray());

            // Act
            var actionResult = controller.GetMessagesByThreadId(threadId);

            // Assert
            Assert.NotNull(actionResult);
            Assert.Equal(2, actionResult.Value.Count);

            // Verify message content
            Assert.Equal("Message 1", actionResult.Value[0].Content.Text);
            Assert.Equal("Message 3", actionResult.Value[1].Content.Text);

            // Verify threadId
            Assert.All(actionResult.Value, msg => Assert.Equal(threadId, msg.ThreadId));
        }



    }
}
