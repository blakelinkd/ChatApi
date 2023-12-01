using System;
using Microsoft.Extensions.Logging;
using Xunit;
using Moq;
using ChatApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System.Text.Json;

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
        public void Get_ReturnsHelloWorld()
        {
            // Arrange
            var controller = new ChatbotController(loggerMock.Object, redisMultiplexerMock.Object);

            // Act
            var result = controller.Get();

            // Assert
            Assert.IsType<string>(result);
            Assert.Equal("Hello World", result);
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

            // Create test messages as serialized JSON strings
            var testMessages = new List<RedisValue>
    {
        JsonSerializer.Serialize(new Message { Content = new MessageContent { Text = "Message 1" } }),
        JsonSerializer.Serialize(new Message { Content = new MessageContent { Text = "Message 2" } }),
        JsonSerializer.Serialize(new Message { Content = new MessageContent { Text = "Message 3" } })
    };

            // Mock the Redis database to simulate messages in the queue
            redisDbMock.Setup(db => db.ListLength("messageQueue", CommandFlags.None)).Returns(() => testMessages.Count);
            redisDbMock.Setup(db => db.ListRightPop("messageQueue", CommandFlags.None))
                       .Returns(() =>
                       {
                           var message = testMessages.LastOrDefault();
                           if (testMessages.Count > 0)
                           {
                               testMessages.RemoveAt(testMessages.Count - 1);
                           }
                           return message;
                       });

            // Act
            var actionResult = controller.GetMessageQueue();

            // Assert
            Assert.NotNull(actionResult);
            var okResult = actionResult.Result as OkObjectResult;
            Assert.NotNull(okResult);
            var resultValue = Assert.IsType<List<Message>>(okResult.Value);
            Assert.Equal(3, resultValue.Count);
            // Additional assertions as needed
        }




    }
}
