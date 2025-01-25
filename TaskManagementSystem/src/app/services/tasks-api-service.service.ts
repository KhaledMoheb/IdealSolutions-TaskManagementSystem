import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { DBTask } from '../models/db-task.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class TasksApiService {
  private baseUrl = `${environment.apiUrl}/Task`; // Replace with actual API URL

  constructor(private http: HttpClient) {}

  // Get all tasks
  getTasks(): Observable<DBTask[]> {
    return this.http.get<DBTask[]>(this.baseUrl);
  }

  getUserTasks(userId: number): Observable<DBTask[]> {
    return this.http.get<DBTask[]>(`${this.baseUrl}/user/${userId}`);
  }

  // Get task by ID
  getTaskById(taskId: number): Observable<DBTask> {
    return this.http.get<DBTask>(`${this.baseUrl}/${taskId}`);
  }

  // Add a new task
  addTask(task: Partial<DBTask>): Observable<DBTask> {
    return this.http.post<DBTask>(this.baseUrl, task);
  }

  // Update a task
  updateTask(task: Partial<DBTask>): Observable<DBTask> {
    return this.http.put<DBTask>(`${this.baseUrl}/${task.id}`, task);
  }

  // Delete a task
  deleteTask(taskId: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${taskId}`);
  }
}
