import { Message } from '../models/message.model';

export interface AppState {
  chat: {
    messages: Message[];
  };
}

export const initialState: AppState = {
  chat: {
    messages: []
  }
};