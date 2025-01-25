import { NgFor, TitleCasePipe } from '@angular/common';
import { Component } from '@angular/core';
import { HeaderComponent } from '../../components/header/header.component';
import { TasksApiService } from '../../services/tasks-api-service.service';
import { AuthService } from '../../services/auth.service';
import { Router } from '@angular/router';
import { DBTask } from '../../models/db-task.model';

@Component({
  selector: 'app-user-dashboard',
  standalone: true,
  imports: [NgFor, TitleCasePipe, HeaderComponent],
  templateUrl: './user-dashboard.component.html',
  styleUrls: ['./user-dashboard.component.scss'],
})
export class UserDashboardComponent {
  tasks: DBTask[] = [];
  username: string;
  
  constructor(
    private tasksService: TasksApiService,
    private authService: AuthService,
    private router: Router
  ) {
    this.username = this.authService.getCurrentUser()!.userName;
  }

  ngOnInit(): void {
    this.getMyTasks();
  }

  // Fetch my tasks
  getMyTasks(): void {
    this.tasksService.getUserTasks(this.authService.getUserId()).subscribe({
      next: (data) => (this.tasks = data),
      error: (err) => console.error('Error fetching tasks', err),
    });
  }

  handleLogout(): void {
    this.authService.logout(); // Clear the token or session
    this.router.navigate(['/login']); // Redirect to login page
  }

  // Updates the status of a task
  updateTaskStatus(task: any): void {
    if (task.status === 'pending') {
      task.status = 'in-progress';
    } else if (task.status === 'in-progress') {
      task.status = 'completed';
    } else {
      task.status = 'pending';
    }
    this.tasksService.updateTask(task).subscribe({
      next: (data) => () => {
        // Update the local tasks array
        this.tasks = this.tasks.map((t) =>
          t.id === task.id ? { ...t, status: data.status } : t
        );
      },
      error: (err) => console.error('Error fetching tasks', err),
    });
  }
}
