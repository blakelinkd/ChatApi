import { OnInit, Component, ViewChildren, QueryList, ElementRef, AfterViewChecked } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Store } from '@ngrx/store';
import { v4 as uuidv4 } from 'uuid';
import { Message } from '../models/message.model';
import * as MessageActions from '../store/message/message.actions';
import { AppState } from '../store/app.state'; // Add this import statement
import { interval, switchMap } from 'rxjs';
import { environment } from '../../environments/environment';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { map } from 'rxjs/operators';

@Component({
  selector: 'app-chatbot',
  templateUrl: './chatbot.component.html',
  styleUrls: ['./chatbot.component.css']
})
export class ChatbotComponent implements OnInit, AfterViewChecked {
  @ViewChildren('message') messageElements!: QueryList<ElementRef>;
  private userHasScrolled = false;
  messages: Message[] = [];
  newMessage: string = '';
  userName: string = '';
  threadId: any = '';
  systemMessageSent: boolean = false;
  messageForm!: FormGroup; // Add the definite assignment assertion operator (!) to indicate that the property will be initialized in the constructor
  nameForm!: FormGroup; // Add the definite assignment assertion operator (!) to indicate that the property will be initialized in the constructor
  title = 'chatbot';

  constructor(private fb: FormBuilder, private http: HttpClient, private store: Store<AppState>) {
    this.threadId = localStorage.getItem('threadId') || uuidv4();
    this.systemMessageSent = false;
    this.messageForm = this.fb.group({
      newMessage: ['', Validators.required]
    });

    this.nameForm = this.fb.group({
      userName: ['', Validators.required]
    });

  }

  ngAfterViewChecked() {
    this.scrollToBottom();
  }

  scrollToBottom(): void {
    const messagesContainer = document.querySelector('.messages');
    if (messagesContainer) {
      messagesContainer.scrollTop = messagesContainer.scrollHeight;
    }
  }

  ngOnInit() {
    const messagesContainer = document.querySelector('.messages');
    if (messagesContainer) {
      messagesContainer.addEventListener('scroll', () => {
        this.userHasScrolled = true;
      });
      if (!this.systemMessageSent) {
        this.addSystemMessage('Welcome! You can set your username by typing "/name [your desired name]".');
        this.systemMessageSent = true;
      }
    }

    console.info("User has entered the chatbot component")
    if (!localStorage.getItem('senderId')) {
      const senderId = uuidv4();
      localStorage.setItem('senderId', senderId);
    }
    this.pollMessages();

    this.store.select((state: any) => state.message).subscribe(messages => {
      this.messages = [...messages].reverse();


    });
  }

  getUserIdColor(userId: string): string {
    let sum = 0;
    for (let i = 0; i < userId.length; i++) {
      sum += userId.charCodeAt(i);
    }

    // Generate red, green, and blue components in the range 0-255
    const red = sum % 256;
    const green = (sum * 2) % 256;
    const blue = (sum * 3) % 256;

    // Ensure that the sum of the components is above a threshold (384 in this case)
    if (red + green + blue < 384) {
      return this.getUserIdColor(userId + 'a');
    }

    // Convert the components to hexadecimal strings and pad them with zeros if necessary
    return '#' + red.toString(16).padStart(2, '0') + green.toString(16).padStart(2, '0') + blue.toString(16).padStart(2, '0');
  }

  sendMessage() {
    console.info("User has sent a message");
    if (this.messageForm.valid) {
      this.newMessage = this.messageForm.value.newMessage;
      this.messageForm.reset();
    } else {
      alert('Please enter a message');
      return;
    }
  
    const senderId = localStorage.getItem('senderId') || uuidv4();
    const userName = localStorage.getItem('userName') || '';
    
    // Adjust the object to use camelCase properties to match the TypeScript `Message` model
    const message: Message = { // Ensure this `Message` type is imported at the top of your file
      messageId: uuidv4(),
      senderId: senderId,
      content: {
        text: this.newMessage,
        attachments: [] // Assuming your `Message` model defines `content` that includes `text` and possibly `attachments`
      },
      timestamp: new Date().toISOString(),
      threadId: this.threadId,
      userName: userName,
      recipientId: '',
      status: '',
      responseTo: '',
      // Ensure any additional required properties by your `Message` type are included here
    };
  
    this.store.dispatch(MessageActions.createMessage({ message }));
  
    this.http.post(environment.postEndpoint, message).subscribe({
      next: (response) => {
        console.log('Message sent successfully', response);
      },
      error: (error) => {
        console.error('Error sending message:', error);
      }
    });
  
    // Clear the newMessage field
    this.newMessage = '';
  }
  

  addSystemMessage(content: string) {
    const message: Message = {
      messageId: uuidv4(),
      senderId: 'system',
      content: {
        text: content,
        attachments: []
      },
      timestamp: new Date().toISOString(),
      threadId: this.threadId,
      userName: 'System',
      recipientId: '',
      status: '',
      responseTo: ''
    };
    this.store.dispatch(MessageActions.createMessage({ message }));
    this.http.post(environment.postEndpoint, message).subscribe();
  }

  pollMessages() {
    interval(1000)
      .pipe(
        switchMap(() => this.http.get<Message[]>(environment.getEndpoint)),
        map((messages: Message[]) => {
          // Separate the messages from the 'System' user and other users
          const systemMessages = messages.filter(message => message.userName === 'System');
          const otherMessages = messages.filter(message => message.userName !== 'System');

          // If there are no system messages, return the other messages
          if (systemMessages.length === 0) {
            return otherMessages;
          }

          // Find the most recent system message
          const mostRecentSystemMessage = systemMessages.reduce((prev, current) => {
            return (new Date(prev.timestamp) > new Date(current.timestamp)) ? prev : current;
          });

          // Combine the most recent system message with the other messages
          return [...otherMessages, mostRecentSystemMessage];
        })
      )
      .subscribe((messages: Message[]) => {
        this.store.dispatch(MessageActions.loadMessages({ messages }));
      });
  }

}