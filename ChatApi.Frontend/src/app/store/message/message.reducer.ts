import { createReducer, on } from '@ngrx/store';
import { Message } from '../../models/message.model'; // Adjust the path according to your project structure
import * as MessageActions from './message.actions';

export const initialState: Message = {
  messageId: '',
  threadId: '',
  senderId: '',
  userName: '',
  recipientId: '',
  timestamp: '',
  content: {
    text: '',
    attachments: []
  },
  status: '',
  responseTo: ''
};

export const messageReducer = createReducer(
  initialState,
  on(MessageActions.createMessage, (state, { message }) => message),
  on(MessageActions.updateMessage, (state, { message }) => ({ ...state, ...message })),
  on(MessageActions.deleteMessage, (state, { messageId }) => initialState)
);