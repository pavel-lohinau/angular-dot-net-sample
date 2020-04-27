import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { UserLogin } from '../app/shared/models/userLogin';
import { DecodeJwt, SaveToken, GetToken, DeleteToken } from './auth.helper';
import { User } from '../app/shared/models/user';
import { BehaviorSubject, Observable } from 'rxjs';
import { Router } from '@angular/router';
import { map, tap } from 'rxjs/operators';

@Injectable({ providedIn: 'root' })
export class AuthenticationService {
  private readonly JWT_TOKEN = 'JWT_TOKEN';
  private readonly REFRESH_TOKEN = 'REFRESH_TOKEN';
  private loggedUser: string;

  private currentUserSubject: BehaviorSubject<User>;
  public currentUser: Observable<User>;

  constructor(private http: HttpClient, private router: Router) {
    this.currentUserSubject = new BehaviorSubject<User>(this.getUserData());
    this.currentUser = this.currentUserSubject.asObservable();
  }

  public get currentUserValue(): User {
    return this.currentUserSubject.value;
  }

  public login(requestModel: UserLogin): Observable<any> {
    return this.http.post<any>('/api/account/login', requestModel)
      .pipe(map(user => {
        this.setSession(user);
      }));
  }

  isLoggedIn() {
    return !!GetToken('Authorization');
  }

  private setSession(token: any): void {
    const payload: User = DecodeJwt(token.token);
    this.currentUserSubject.next(payload);
    SaveToken('Authorization', token.token);
    SaveToken('RefreshToken', token.refreshToken);
  }

  public getUserData(): User {
    const token: string = GetToken('Authorization');
    if (token) {
      const user: User = DecodeJwt(token);
      return user;
    }

    return null;
  }

  public async refreshToken(): Promise<Observable<any>> {
    const requestModel = {
      token: GetToken('Authorization'),
      refreshToken: GetToken('RefreshToken')
    };

    return this.http.post<any>('/api/token/refresh', requestModel);
  }

  public logout(): void {
    const requestModel = {
      token: GetToken('Authorization'),
      refreshToken: GetToken('RefreshToken')
    };

    this.http.post<any>('/api/token/revoke', requestModel).subscribe(() => {
      this.removeAuthData();
    },
      () => {
        this.removeAuthData();
      });
  }

  private removeAuthData(): void {
    DeleteToken('Authorization');
    DeleteToken('RefreshToken');
    this.currentUserSubject.next(null);
    this.router.navigate(['/']);
  }
}
