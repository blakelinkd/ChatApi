import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { Subject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class WebSocketService {
  private hubConnection: HubConnection;
  private messageSubject = new Subject<string>();

  constructor() {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl('https://localhost:6379') // Replace with your server's WebSocket endpoint
      .build();

    this.hubConnection.start().catch(err => console.error(err));

    this.hubConnection.on('ReceiveMessage', (message: string) => {
      // Handle incoming messages
      this.messageSubject.next(message);
    });
  }

  sendMessage(message: string) {
    this.hubConnection.invoke('SendMessage', message);
  }

  getMessageSubject() {
    return this.messageSubject.asObservable();
  }
}
