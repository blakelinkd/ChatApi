import { createAction, props } from '@ngrx/store';
import { Message, Attachment } from '../../models/message.model'; // Adjust the path according to your project structure

export const loadMessages = createAction(
  '[Message] Load Messages',
  props<{ messages: Message[] }>()
);

export const createMessage = createAction(
  '[Message] Create Message',
  props<{ message: Message }>()
);

export const updateMessage = createAction(
  '[Message] Update Message',
  props<{ message: Message }>()
);

export const deleteMessage = createAction(
  '[Message] Delete Message',
  props<{ messageId: string }>()
);
