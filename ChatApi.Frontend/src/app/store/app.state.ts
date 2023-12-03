import { Message } from '../models/message.model';

export interface AppState {
  chat: {
    messages: Message[];
  };
}