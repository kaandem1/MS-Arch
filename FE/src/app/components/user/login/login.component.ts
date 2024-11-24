import { Component, HostListener } from '@angular/core';
import { AuthService } from '../../../services/Authorization/auth.service';
import { UserLogin } from '../../../interfaces/UserLogin';
import { NgForm } from '@angular/forms';
import { Router } from '@angular/router';


@Component({
  selector: 'login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent {
  username: string = '';
  password: string = '';
  pupils!: NodeListOf<HTMLElement>;

  constructor(private authService: AuthService, private router: Router) { }

  login(form: NgForm) {
    if (form.valid) {
      const loginData: UserLogin = {
        username: this.username,
        password: this.password
      };
      this.authService.login(loginData).subscribe({
        next: token => {
          this.authService.handleLoginSuccess(token);
        },
        error: error => {
        }
      });
    } else {
    }
  }

  navigateToRegister() {
    this.router.navigate(['/register']);
  }

  navigateToForgotPassword() {
    this.router.navigate(['/reset-password']);
  }

ngAfterViewInit() {
  this.pupils = document.querySelectorAll(".eye .pupil");
}
  

 @HostListener('mousemove', ['$event'])
  onMouseMove(event: MouseEvent): void {
    this.pupils.forEach((pupil) => {
      const rect = pupil.getBoundingClientRect();
      const x = ((event.pageX - rect.left) / 30) + 'px';
      const y = ((event.pageY - rect.top) / 30) + 'px';
      if(document.activeElement=== document.getElementById("password"))
        pupil.style.transform=`translate3d(${2.6}px,${14.27}px, 0px)`
      else
      pupil.style.transform = `translate3d(${x}, ${y}, 0px)`;
    });
  }
}


  
 
 

