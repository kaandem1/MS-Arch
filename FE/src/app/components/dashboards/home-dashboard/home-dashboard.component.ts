import { Component, OnInit } from '@angular/core';
import { Observable, map } from 'rxjs';
import { Router } from '@angular/router';
import { JWTTokenService } from '../../../services/JWTToken/jwttoken.service';
import { UtilityService } from '../../../services/Utility/utility.service';


@Component({
  selector: 'home-dashboard',
  templateUrl: './home-dashboard.component.html',
  styleUrls: ['./home-dashboard.component.scss'],
})
export class HomeDashboardComponent implements OnInit {

  date: number | null = null;
  display: boolean = false;

  isAdmin: boolean = true;

  constructor(
    private router: Router,
    private jwtService: JWTTokenService,
    private utilityService: UtilityService
  ) {}

  ngOnInit(): void {
    this.checkIfAdmin();
  }
  


  checkIfAdmin(): void {
    const role = this.jwtService.getUserRole();
    this.isAdmin = this.utilityService.checkIfAdmin();
  }



  
}
