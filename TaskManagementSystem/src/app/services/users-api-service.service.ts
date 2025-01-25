import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { DBUser } from '../models/db-user.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class UsersApiService {
  private baseUrl = `${environment.apiUrl}/user`; // Replace with actual API URL

  constructor(private http: HttpClient) {}

  getUsers(): Observable<DBUser[]> {
    console.log('UsersApiService getUsers baseUrl', this.baseUrl);
    return this.http.get<DBUser[]>(`${this.baseUrl}`);
  }

  getUserById(userId: number): Observable<DBUser> {
    return this.http.get<DBUser>(`${this.baseUrl}/${userId}`);
  }

  addUser(user: Partial<DBUser>): Observable<DBUser> {
    return this.http.post<DBUser>(this.baseUrl, user);
  }

  updateUser(userId: number, user: Partial<DBUser>): Observable<DBUser> {
    return this.http.put<DBUser>(`${this.baseUrl}/${userId}`, user);
  }

  deleteUser(userId: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${userId}`);
  }
}
