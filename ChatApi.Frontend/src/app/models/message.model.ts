export interface Attachment {
    type: string;
    url: string;
  }
  
  export interface Message {
    messageId: string;
    threadId: string;
    senderId: string;
    userName: string;
    recipientId: string;
    timestamp: string;
    content: {
      text: string;
      attachments: Attachment[];
    };
    status: string;
    responseTo: string;
  }