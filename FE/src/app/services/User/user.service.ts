import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';
import { User } from '../../interfaces/User';
import { UserCreate } from '../../interfaces/UserCreate';
import { UserShow } from '../../interfaces/UserShow';
import { baseUrl, userEndpoint } from '../../constants/api-constants';
import { JWTTokenService } from '../JWTToken/jwttoken.service';
import { UserResetPassword } from '../../interfaces/UserResetPassword';
@Injectable({
  providedIn: 'root'
})
export class UserService {

  private url = baseUrl + userEndpoint;
  constructor(private http: HttpClient) { }

  getAllUsers(): Observable<User[]> {
    return this.http.get<User[]>(`${this.url}`);
  }
  getUserById(id: number): Observable<User> {
    return this.http.get<User>(`${this.url}/${id}`);
  }
  
  createUser(user: UserCreate): Observable<User> {
    return this.http.post<User>(`${this.url}`, user);
  }

updateUser(userId: number, userUpdateDTO: UserShow): Observable<any> {
  const urll = `${this.url}/${userId}`;
  return this.http.put(urll, userUpdateDTO);
}

  deleteUser(id: number): Observable<void> {
    return this.http.delete<void>(`${this.url}/${id}`);
  }
  
  inactivateUser(id: number, status: number): Observable<void> {
    return this.http.patch<void>(`${this.url}/${id}/inactivate`, status);
  }

  changeRole(id:number, role: number): Observable<void> {
    return this.http.patch<void>(`${this.url}/${id}/change-role`, role);
  }
  searchUser(name:string): Observable<User[]> {
    return this.http.get<User[]>(`${this.url}/search?name=${name}`);
  }

  resetPassword(user: UserResetPassword): Observable<any> {
    return this.http.post<UserResetPassword>(`${this.url}/reset-password`, user);
  }
}
