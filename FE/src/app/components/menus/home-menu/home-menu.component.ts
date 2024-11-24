import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { JWTTokenService } from '../../../services/JWTToken/jwttoken.service';
import { UtilityService } from '../../../services/Utility/utility.service';


@Component({
  selector: 'home-menu',
  templateUrl: './home-menu.component.html',
  styleUrls: ['./home-menu.component.scss']
})
export class HomeMenuComponent implements OnInit{
  isAdmin: boolean = false;
  userId?: string;
  constructor(private jwtService: JWTTokenService, private router: Router, private utilityService: UtilityService) {}

  ngOnInit(){
    this.checkIfAdmin();
    this.userId = this.jwtService.getUserId();
  }
  logout() {
    this.jwtService.clearToken();
    this.router.navigate(['/login']);
    
  }

  navigateToUserList() {
    this.router.navigate(['/user-list']);
  }
  
  navigateToMyDevices() {
    if (this.userId) {
      this.router.navigate(['/my-devices', this.userId]);
    }
  }
  
  checkIfAdmin(): void {
    const role = this.jwtService.getUserRole();
    this.isAdmin = this.utilityService.checkIfAdmin();
  }
}
