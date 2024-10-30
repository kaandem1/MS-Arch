import { Component, HostListener } from '@angular/core';
import { NgForm } from '@angular/forms';
import { UserService } from '../../../services/User/user.service';
import { MessageService } from 'primeng/api';
@Component({
  selector: 'register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent {
  
  confirmPassword: string = '';
  submitted: boolean = false; 
  pupils!: NodeListOf<HTMLElement>;
  constructor(private userService: UserService,private messageService:MessageService) { }

  onSubmit(registerForm: NgForm): void {
    if (registerForm.valid) {
        this.userService.createUser(registerForm.value).subscribe(user => {
          this.messageService.add({ severity: 'success', summary: 'Success', detail: 'User created!!' }); 
          this.submitted = true;
        },
        error => {
          this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Error creating user!!' }); 
        }
      );
       }
    }

  resetForm(): void {
    this.submitted = false;
    this.confirmPassword = '';
  }
    
}
