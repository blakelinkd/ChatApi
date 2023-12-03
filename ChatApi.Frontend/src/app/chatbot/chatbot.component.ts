import { Component, NgModule, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Store } from '@ngrx/store';
import { v4 as uuidv4 } from 'uuid';
import { Message } from '../models/message.model';
import * as MessageActions from '../store/message/message.actions';
import { AppState } from '../store/app.state'; // Add this import statement


@Component({
  selector: 'app-chatbot',
  templateUrl: './chatbot.component.html',
  styleUrls: ['./chatbot.component.css']
})
export class ChatbotComponent implements OnInit {
  messages: Message[] = [];
  newMessage: string = '';
  userName: string = '';
  apiUrl = 'your-api-endpoint';
  title = 'chatbot';

  constructor(private http: HttpClient, private store: Store<AppState>) {}
  ngOnInit() {
    // Dispatch the action to load messages from the API
    this.store.dispatch(MessageActions.loadMessages());
    
    this.store.select((state: any) => state.chat.messages).subscribe(messages => {
      this.messages = messages;
    });
  }

  setUserName() {
    this.userName = this.userName;
  }

  sendMessage() {
    const senderId = localStorage.getItem('senderId') || uuidv4();

    if (!localStorage.getItem('senderId')) {
      localStorage.setItem('senderId', senderId);
    }

    const message: Message = {
      messageId: uuidv4(),
      senderId: senderId,
      content: {
        text: this.newMessage,
        attachments: [] // Set attachments to an empty array if there are no attachments
      },
      timestamp: new Date().toISOString() // Use the current date and time as the timestamp
      ,
      threadId: '',
      userName: '',
      recipientId: '',
      status: '',
      responseTo: ''
    };

    // Dispatch the action to create a new message
    this.store.dispatch(MessageActions.createMessage({ message }));

    // Clear the newMessage property to reset the input field
    this.newMessage = '';
  }
}