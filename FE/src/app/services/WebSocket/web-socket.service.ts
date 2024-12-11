import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class WebSocketService {
    private socket: WebSocket;
    private notificationSubject: BehaviorSubject<string> = new BehaviorSubject<string>('');

    notification$ = this.notificationSubject.asObservable();

    constructor() {
      this.socket = new WebSocket('ws://localhost:80/ws');
        this.socket.onmessage = (event) => {
            const notification = event.data;
            this.handleNotification(notification);
        };
    }

    private handleNotification(notification: string) {
        this.notificationSubject.next(notification);
    }
}
