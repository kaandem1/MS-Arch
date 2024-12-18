import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { BehaviorSubject } from 'rxjs';
import { JWTTokenService } from '../JWTToken/jwttoken.service';
import { Subject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class SignalRService {
  private hubConnection: signalR.HubConnection;
  public messages$: BehaviorSubject<any[]> = new BehaviorSubject<any[]>([]);
  public typing$: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
  public lastSeenIndex$: BehaviorSubject<{ lastSeenIndex: number | null; seenBy: string | null }> = 
  new BehaviorSubject<{ lastSeenIndex: number | null; seenBy: string | null }>({ lastSeenIndex: null, seenBy: null });
  public endConversation$: BehaviorSubject<void> = new BehaviorSubject<void>(undefined);
  public userUpdate$: BehaviorSubject<{ userId: string; userName: string; isConnected: boolean }> = 
  new BehaviorSubject<{ userId: string; userName: string; isConnected: boolean }>({
    userId: '',
    userName: '',
    isConnected: false
  });

  adminConnectionStatus$ = new Subject<boolean>();

  public connectedUsers$: BehaviorSubject<{ userId: string; userName: string }[]> = 
  new BehaviorSubject<{ userId: string; userName: string }[]>([]);


  constructor(private jwtTokenService: JWTTokenService) {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('http://localhost/chat', {
        accessTokenFactory: () => {
          const token = this.jwtTokenService.getJWTToken();
          if (!token) throw new Error('Token is null or undefined');
          return token;
        }
      })
      .withAutomaticReconnect()
      .build();
  }

  startConnection(): void {
    this.hubConnection
      .start()
      .then(() => console.log('SignalR connection started'))
      .catch(err => console.error('Error while starting SignalR connection:', err));

      this.hubConnection.on('AdminConnectionStatus', (status: boolean) => {
        this.adminConnectionStatus$.next(status);
      });

      this.hubConnection.on('ReceiveMessage', (message) => {
        const currentMessages = this.messages$.value;
      
        const exists = currentMessages.some(
          (m) => m.text === message.text && m.sender === message.sender
        );
      
        if (!exists) {
          this.messages$.next([...currentMessages, message]);
        }
      });
      

      this.hubConnection.on('MessagesSeen', (data: { lastSeenIndex: number; seenBy: string; seenByName: string }) => {
        console.log(`Message seen: Last index: ${data.lastSeenIndex}, By: ${data.seenByName}`);
      
        this.lastSeenIndex$.next({
          lastSeenIndex: data.lastSeenIndex,
          seenBy: data.seenByName
        });
      });
      

    this.hubConnection.on('EndConversation', () => {
      console.log('Conversation ended signal received.');
      this.endConversation$.next();
    });
  
    this.hubConnection.on('ConnectedUsersList', (users: { userId: string; userName: string }[]) => {
      console.log('Connected users list:', users);
      this.connectedUsers$.next(users);
    });
    
  
    this.hubConnection.on('TypingNotification', (sender, isTyping) => {
      this.typing$.next(isTyping);
    });

    this.hubConnection.on('UserUpdate', (update: { UserId: string; UserName: string; IsConnected: boolean }) => {
      console.log('User update:', update);
      this.userUpdate$.next({
        userId: update.UserId,
        userName: update.UserName,
        isConnected: update.IsConnected
      });
    });
    
  }

  sendMessage(text: string, receiver: string): void {
    const message = { text, sender: this.jwtTokenService.getUserId(), receiver };
    this.hubConnection
      .invoke('SendMessage', message)
      .catch(err => console.error('Error while sending message:', err));
  }

  markMessagesAsSeen(sender: string, lastSeenIndex: number, seenBy: string): void {
    if (!sender) {
      console.error('Sender is null or undefined');
      return;
    }
  
    this.hubConnection
      .invoke('MarkMessagesAsSeen', sender, lastSeenIndex, seenBy)
      .catch(err => console.error('Error while marking messages as seen:', err));
  }


  notifyTyping(receiver: string, isTyping: boolean): void {
    this.hubConnection
      .invoke('NotifyTyping', receiver, isTyping)
      .catch(err => console.error('Error while sending typing notification:', err));
  }

  endConversation(): void {
    this.hubConnection
      .invoke('EndConversation')
      .catch((err) => console.error('Error while ending conversation:', err));
  }
}
