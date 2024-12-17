import { Component, OnInit } from '@angular/core';
import { SignalRService } from '../../services/SignalR/signalr.service';
import { JWTTokenService } from '../../services/JWTToken/jwttoken.service';

@Component({
  selector: 'app-chat',
  templateUrl: './chat.component.html',
  styleUrls: ['./chat.component.scss']
})
export class ChatComponent implements OnInit {
  message = '';
  messages: any[] = [];
  isTyping = false;
  selectedReceiver = '';
  userList: Map<string, string> = new Map();
  lastSeenMessageIndex: number | null = null;
  seenBy: string | null = null;
  seenTriggered: boolean = false;
  lastSeenIndexes: Map<string, number> = new Map();

  constructor(
    private signalRService: SignalRService,
    private jwtService: JWTTokenService
  ) {}

  ngOnInit(): void {
    this.signalRService.startConnection();
  
    this.signalRService.connectedUsers$.subscribe((users) => {
      users.forEach((user) => this.userList.set(user.userId, user.userName));
    });
  
    this.signalRService.userUpdate$.subscribe(({ userId, isConnected, userName }) => {
      if (isConnected) {
        this.userList.set(userId, userName);
      } else {
        this.userList.delete(userId);
        if (this.selectedReceiver === userId) {
          this.clearChat();
          this.selectedReceiver = '';
          console.log(`User ${userName} disconnected, clearing chat.`);
        }
      }
    });
    this.signalRService.messages$.subscribe((newMessages) => {
      newMessages.forEach((msg) => {
        const exists = this.messages.some(
          (m) => m.text === msg.text && m.sender === msg.sender
        );
        if (!exists) {
          this.messages.push({
            ...msg,
            senderName: msg.senderName || this.userList.get(msg.sender) || msg.sender
          });
        }
      });
      this.seenTriggered = false;
    });
  
    this.signalRService.endConversation$.subscribe(() => {
      console.log('Conversation ended.');
      this.clearChat();
    });
  
    this.signalRService.typing$.subscribe((isTyping) => {
      this.isTyping = isTyping;
    });
  
    this.signalRService.lastSeenIndex$.subscribe(({ lastSeenIndex, seenBy }) => {
      if (lastSeenIndex !== null && seenBy) {
        this.lastSeenIndexes.set(seenBy, lastSeenIndex);
      }
    });
  }

  ngOnChanges(): void {
    this.clearChat();
    console.log('Switching conversation, clearing previous chat.');
  }  

  clearChat(): void {
    this.messages = [];
    console.log('Chat cleared.');
  }

  sendMessage(): void {
    if (this.message.trim()) {
      if (this.isAdmin() && (!this.selectedReceiver || !this.userList.has(this.selectedReceiver))) {
        console.error('No valid user selected to send a message to.');
        return;
      }

      const receiver = this.isAdmin() ? this.selectedReceiver : 'admin';
      const newMessage = {
        text: this.message,
        sender: this.getUserId(),
        senderName: this.getUserName(),
        receiver
      };

      this.messages.push(newMessage);
      this.signalRService.sendMessage(this.message, receiver);
      this.message = '';

      this.seenTriggered = false;
    }
  }

  notifyTyping(isTyping: boolean): void {
    const receiver = this.isAdmin() ? this.selectedReceiver : 'admin';
    if (receiver) {
      this.signalRService.notifyTyping(receiver, isTyping);
    }
  }

  getUserId(): string | undefined {
    return this.jwtService.getUserId();
  }

  getUserName(): string | undefined {
    return this.jwtService.getUserName();
  }

  isAdmin(): boolean {
    return this.getUserId() === '1';
  }

  focusInput(): void {
    if (this.seenTriggered) {
      return;
    }

    const sender = this.isAdmin() ? this.selectedReceiver : this.getUserId();
    const lastMessage = this.messages[this.messages.length - 1];

    if (lastMessage && lastMessage.sender !== this.getUserId()) {
      const lastSeenIndex = this.messages.length - 1;
      this.signalRService.markMessagesAsSeen(sender!, lastSeenIndex, this.getUserId()!);

      this.seenTriggered = true;
    }
  }
}
