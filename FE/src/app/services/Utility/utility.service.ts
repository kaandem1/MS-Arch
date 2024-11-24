import { Injectable } from '@angular/core';
import { Location } from '@angular/common';
import { JWTTokenService } from '../JWTToken/jwttoken.service';
import { UserRole } from '../../enums/user-roles.enum';

@Injectable({
  providedIn: 'root'
})
export class UtilityService {

  constructor(private location: Location, private jwtService: JWTTokenService) { }

  checkIfAdmin(): boolean {
    const role = this.jwtService.getUserRole();
    return role === UserRole.ADMIN;
  }
}
