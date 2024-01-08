namespace ChatApi.Models
{
    public class Message
    {
        public string MessageId { get; set; }
        public string ThreadId { get; set; }
        public string SenderId { get; set; }
        public string userName { get; set; }
        public string RecipientId { get; set; }
        public DateTime Timestamp { get; set; }

        // Flattened properties from MessageContent
        public string Text { get; set; } // Assuming you only need the text

        // Handling Attachments
        // If you need to preserve the list of attachments, you have several options:
        // Option 1: Serialize the list of attachments into a single string
        public string AttachmentsJson { get; set; }

        // Option 2: Store only the first attachment, if that's sufficient for your use case
        public string FirstAttachmentType { get; set; }
        public string FirstAttachmentUrl { get; set; }

        // ... other properties from Message
        public string Status { get; set; }
        public string ResponseTo { get; set; }
    }

    // If you choose to serialize the attachments, you don't need the Attachment class.
    // If you choose to store the first attachment only, then also you won't need the Attachment class.
}
