import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { LoginResponse } from '../models/login-response.model';
import { DBUser } from '../models/db-user.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private baseUrl = `${environment.apiUrl}/Login`; // Replace with actual API URL
  private currentUser: Omit<DBUser, 'password'> | null = null;

  constructor(private http: HttpClient) {
    this.loadUser();
  }

  // Login method
  login(Username: string, password: string): Observable<LoginResponse> {
    console.log('Username', Username);
    console.log('password', password);
    return this.http.post<LoginResponse>(`${this.baseUrl}/login`, {
      Username,
      password,
    });
  }

  // Logout method
  logout(): void {
    this.http.post(`${this.baseUrl}/logout`, {}).subscribe(() => {
      this.currentUser = null;
      localStorage.removeItem('user');
      localStorage.removeItem('authToken');
    });
  }

  // Save token to local storage
  setToken(token: string): void {
    localStorage.setItem('authToken', token);
  }

  // Get token from local storage
  getToken(): string | null {
    return localStorage.getItem('authToken');
  }

  // Check if user is authenticated
  isAuthenticated(): boolean {
    return !!this.getToken();
  }

  // Save user to local storage (called after successful login)
  setUser(user: Omit<DBUser, 'password'>): void {
    localStorage.setItem('user', JSON.stringify(user));
    this.currentUser = user;
  }

  // Load user from local storage
  loadUser(): void {
    const userData = localStorage.getItem('user');
    if (userData) {
      this.currentUser = JSON.parse(userData) as DBUser;
    } else {
      this.currentUser = null;
    }
  }

  // Get user role
  getUserId(): number {
    return this.currentUser ? this.currentUser.id : 0;
  }

  // Get user role
  getUserRole(): string {
    return this.currentUser ? this.currentUser.role : '';
  }

  // Get current user
  getCurrentUser(): Omit<DBUser, 'password'> | null {
    return this.currentUser;
  }
}
