import { Injectable } from '@angular/core';
import { CanActivate, Router, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { JWTTokenService } from '../services/JWTToken/jwttoken.service';

@Injectable({
  providedIn: 'root'
})
export class LoginGuard implements CanActivate {

  constructor(private router: Router, private jwtService: JWTTokenService) {}

  canActivate(
    next: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): boolean {
    if (this.isLoggedIn() && !this.jwtService.isTokenExpired()) {
      this.router.navigate(['/home']);
      return false;
    } else {
      return true;
    }
  }

  isLoggedIn(): boolean {
    return !!this.jwtService.getUserId();
  }
}
