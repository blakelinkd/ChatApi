export interface Message {
  messageId: string;
  threadId: string;
  senderId: string;
  userName: string;
  recipientId: string;
  timestamp: string; // The backend expects an ISO string
  text: string; // Flattened property from MessageContent

  // Handling Attachments
  // Option 1: Serialize the list of attachments into a single string
  attachmentsJson?: string;

  // Option 2: Store only the first attachment, if that's sufficient for your use case
  firstAttachmentType?: string;
  firstAttachmentUrl?: string;

  // ... other properties from Message
  status: string;
  responseTo: string;
}
