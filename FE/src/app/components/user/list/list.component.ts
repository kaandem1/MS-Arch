import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { User } from '../../../interfaces/User';
import { UserService } from '../../../services/User/user.service';
import { NavigationStart, Router } from '@angular/router';
import { UserRole } from '../../../enums/user-roles.enum';
import { MessageService } from 'primeng/api';
import { UtilityService } from '../../../services/Utility/utility.service';
@Component({
  selector: 'user-list',
  templateUrl: './list.component.html',
  styleUrl: './list.component.scss'
})
export class ListComponent implements OnInit {
  users: User[] = [];
  user: User | undefined;
  searchTerm: string = '';
  userRoleNames:string[]=[];
  isAdmin:boolean = false;
  @Output() userSelected: EventEmitter<number> = new EventEmitter<number>();
  constructor(private userService: UserService, private router:Router,private messageService:MessageService, private utilityService:UtilityService) { }

  ngOnInit(): void {
    this.isAdmin = this.utilityService.checkIfAdmin();
    if(!this.isAdmin){
      this.router.navigate(['/home']);
    }
    this.loadUsers();
    if (event instanceof NavigationStart) {
      this.messageService.clear();
    }

  }

  loadUsers(): void {
    this.userService.getAllUsers().subscribe(users => {
      this.users = users;
    });
  }

  onSelectUser(id?: number): void {
    this.router.navigate(['/user-details', id]);
  }

  searchUsers(): void {
    if (this.searchTerm.trim() === '') {
      this.loadUsers(); 
    } else {
      this.userService.searchUser(this.searchTerm).subscribe(users => {
        this.users = users;
        this.messageService.add({ severity: 'success', summary: 'Success', detail: 'User found!' }); 
      },
      error=>
      {
        this.messageService.add({ severity: 'error', summary: 'Error', detail: 'User not found!' });
      }
    );
    }
  }
}
