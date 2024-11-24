import { Injectable } from '@angular/core';
import { jwtDecode } from 'jwt-decode';
import { DecodedToken } from '../../interfaces/DecodedToken';
import { AppCookieService } from '../AppCookie/app-cookie.service';
import { UserRole } from '../../enums/user-roles.enum';

@Injectable({
  providedIn: 'root',
})
export class JWTTokenService {
  private decodedToken: DecodedToken | null = null;
  
  constructor(private cookieService: AppCookieService) {
    const token = this.cookieService.get('jwtToken');
    if (token) {
      this.setToken(token);
    }
  }

  setToken(token: string) {
    if (token) {
      this.cookieService.set('jwtToken', token);
      this.decodeToken();
    }
  }

  private decodeToken() {
    try {
      const token = this.cookieService.get('jwtToken');
      if (token) {
        this.decodedToken = jwtDecode<DecodedToken>(token);
      }
    } catch (error) {
      console.error('Failed to decode token', error);
      this.decodedToken = null;
    }
  }

  getDecodedToken(): DecodedToken | null {
    return this.decodedToken;
  }

  getJWTToken(): string | null {
    return this.cookieService.get('jwtToken');
  }

  getUserId(): string | undefined {
    const decodedToken = this.getDecodedToken();
    return decodedToken?.nameid;
  }

  getUserRole(): UserRole | null {
    const role = this.decodedToken?.role;
    return this.mapRoleToUserRole(role);
  }

  clearToken() {
    this.cookieService.remove('jwtToken');
    this.decodedToken = null;
  }
 
  private mapRoleToUserRole(role: UserRole | undefined): UserRole | null {
    switch (role?.toString().toLowerCase()) {
      case 'admin':
        return UserRole.ADMIN;
      case 'guest':
        return UserRole.GUEST;
      default:
        return null;
    }
  }

  isTokenExpired(): boolean {
    const expiryTime = this.getExpiryTime();
    const currentTimeInSeconds = Date.now() / 1000;
    return expiryTime !== null && currentTimeInSeconds > expiryTime;
  }

  getExpiryTime(): number | null {
    return this.decodedToken?.exp ?? null;
  }
}
