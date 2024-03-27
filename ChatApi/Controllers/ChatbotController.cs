using ChatApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace ChatApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ChatbotController : ControllerBase
    {
        private readonly ILogger<ChatbotController> _logger;
        private readonly IDatabase _redisDatabase;

        public ChatbotController(ILogger<ChatbotController> logger, IConnectionMultiplexer redisMultiplexer)
        {
            _logger = logger;

            try
            {
                _redisDatabase = redisMultiplexer.GetDatabase();
                _logger.LogInformation("Connected to Redis successfully.");
            }
            catch (RedisConnectionException ex)
            {
                _logger.LogError("Error connecting to Redis: {Message}", ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError("Unexpected error: {Message}", ex.Message);
                throw;
            }
        }

        [HttpPost("message/post")]
        public IActionResult Post([FromBody] Message message)
        {
            _logger.LogInformation($"Received POST request with payload: {message}");


            if (message == null || string.IsNullOrEmpty(message.Text))
            {
                _logger.LogWarning("Received a bad request with null or empty message text.");
                return BadRequest();
            }

            _logger.LogInformation($"Received message: {message.Text}");

            // Serialize the Message object to a string (JSON) before storing in Redis
            var messageString = System.Text.Json.JsonSerializer.Serialize(message);
            _redisDatabase.ListLeftPush("messageQueue", messageString);

            return Ok();
        }

        // endpoint to say hello
        [HttpGet("hello")]
        public ActionResult<string> Get()
        {
            return "Hello from the Chatbot API!";
        }

        [HttpGet("message/get")]
        public ActionResult<List<Message>> GetMessageQueue()
        {
            var messages = new List<Message>();
            var messageStrings = _redisDatabase.ListRange("messageQueue");

            foreach (var messageString in messageStrings)
            {
                var message = System.Text.Json.JsonSerializer.Deserialize<Message>(messageString);
                if (message != null)
                {
                    messages.Add(message);
                }
            }

            return Ok(messages);
        }

        [HttpGet("message/getBySenderId/{senderId}")]
        public ActionResult<List<Message>> GetMessagesBySenderId(string senderId)
        {
            var messages = new List<Message>();
            var messageStrings = _redisDatabase.ListRange("messageQueue");

            foreach (var messageString in messageStrings)
            {
                var message = System.Text.Json.JsonSerializer.Deserialize<Message>(messageString);
                if (message != null && message.SenderId == senderId)
                {
                    messages.Add(message);
                }
            }

            return messages;
        }

       

    }
}
