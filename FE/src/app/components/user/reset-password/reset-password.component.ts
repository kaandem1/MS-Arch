import { Component } from '@angular/core';
import { NgForm } from '@angular/forms';
import { UserService } from '../../../services/User/user.service';
import { UserResetPassword } from '../../../interfaces/UserResetPassword';
import { NavigationStart, Router } from '@angular/router';
import { MessageService } from 'primeng/api';
import {Message} from 'primeng//api';

@Component({
  selector: 'reset',
  templateUrl: './reset-password.component.html',
  styleUrls: ['./reset-password.component.scss'],
})

export class ResetPasswordComponent {
  userResetPassword : UserResetPassword = {email:'', newPassword:''};
  confirmPassword: string = '';
  submitted: boolean = false; 

  constructor(private router: Router,private userService: UserService,private messageService: MessageService) { }

  onSubmit(registerForm: NgForm): void {
    if (registerForm.valid) {
      this.userResetPassword.email = registerForm.value.email;
      this.userResetPassword.newPassword = registerForm.value.password;

      this.userService.resetPassword(this.userResetPassword).subscribe(
        response => {
          
          this.submitted = true;
          this.messageService.add({ severity: 'success', summary: 'Success', detail: 'Password reset successful!' });
          this.resetForm(); 
          this.router.navigate(['/login']); 
        },
        error => {
          
          this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Password reset failed. Please try again.' });
        }
      );
    }
    if (event instanceof NavigationStart) {
      this.messageService.clear();
    }
    
  }

  resetForm(): void {
    this.submitted = false;
    this.confirmPassword = '';
  }

  goBack() {
    this.router.navigate(['/login']);
  }
}
