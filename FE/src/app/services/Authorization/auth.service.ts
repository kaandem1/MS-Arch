import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { catchError, Observable, throwError } from 'rxjs';
import { Router } from '@angular/router';
import { UserLogin } from '../../interfaces/UserLogin';
import { JWTTokenService } from '../JWTToken/jwttoken.service';
import { AppCookieService } from '../AppCookie/app-cookie.service';
import { User } from '../../interfaces/User';


@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = 'http://localhost';

  constructor(
    private http: HttpClient,
    private cookieService: AppCookieService,
    private jwtService: JWTTokenService,
    private router: Router,
  ) {}

  login(userLogin: UserLogin): Observable<string> {
    const loginUrl = `${this.apiUrl}/api/User/login`;
    return this.http.post(loginUrl, userLogin, { responseType: 'text' }).pipe(
      catchError(this.handleError)
    );
  }

  handleLoginSuccess(token: string) {
    this.jwtService.setToken(token);
    this.router.navigate(['/home']);
  }

  private handleError(error: HttpErrorResponse) {
    console.error('Server returned code:', error.status);
    console.error('Error message:', error.message);
    return throwError(() => new Error('Something went wrong. Please try again later.'));
  }

  getJWTToken(): string | null {
    return this.jwtService.getJWTToken();
  }
}
