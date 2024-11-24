import { Component } from '@angular/core';
import { User } from '../../../interfaces/User';
@Component({
  selector: 'home',
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss'
})
export class HomeComponent {
  selectedUserId: number | null = null;

  handleUserSelected(userId: number): void {
    this.selectedUserId = userId;
  }

}
