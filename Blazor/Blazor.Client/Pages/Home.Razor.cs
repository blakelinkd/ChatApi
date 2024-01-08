
public class ChatMessageRequest
{
    public string MessageId { get; set; }
    public string ThreadId { get; set; }
    public string SenderId { get; set; }
    public string UserName { get; set; }
    public string RecipientId { get; set; }
    public DateTime Timestamp { get; set; }
    public Content Content { get; set; }
    public string Status { get; set; }
    public string ResponseTo { get; set; }
}

public class Content
{
    public string Text { get; set; }
    public List<Attachment> Attachments { get; set; }
}

public class Attachment
{
    public string Type { get; set; }
    public string Url { get; set; }
}
