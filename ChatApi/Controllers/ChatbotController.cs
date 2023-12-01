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
            _redisDatabase = redisMultiplexer.GetDatabase();
        }

        [HttpGet(Name = "chatbot")]
        public String Get()
        {
            return "Hello World";
        }


        [HttpPost(Name = "message")]
        public IActionResult Post([FromBody] Message message)
        {
            if (message == null || string.IsNullOrEmpty(message.Content?.Text))
            {
                return BadRequest();
            }

            _logger.LogInformation(message.Content.Text);

            // Serialize the Message object to a string (JSON) before storing in Redis
            var messageString = System.Text.Json.JsonSerializer.Serialize(message);
            _redisDatabase.ListLeftPush("messageQueue", messageString);

            return Ok();
        }

        [HttpGet("message/get")]
        public ActionResult<List<Message>> GetMessageQueue()
        {
            var messages = new List<Message>();
            while (_redisDatabase.ListLength("messageQueue") > 0)
            {
                var messageString = _redisDatabase.ListRightPop("messageQueue");
                if (!messageString.IsNullOrEmpty)
                {
                    var message = System.Text.Json.JsonSerializer.Deserialize<Message>(messageString);
                    if (message != null)
                    {
                        messages.Add(message);
                    }
                }
            }

            return Ok(messages);
        }
    }
}
