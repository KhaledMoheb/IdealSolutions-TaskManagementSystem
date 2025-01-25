import { Component, OnInit } from '@angular/core';
import { DBUser } from '../../models/db-user.model';
import { DBTask } from '../../models/db-task.model';
import { UsersApiService } from '../../services/users-api-service.service';
import { TasksApiService } from '../../services/tasks-api-service.service';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { HeaderComponent } from '../../components/header/header.component';
import { AuthService } from '../../services/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-admin-dashboard',
  templateUrl: './admin-dashboard.component.html',
  styleUrls: ['./admin-dashboard.component.scss'],
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, HeaderComponent],
})
export class AdminDashboardComponent implements OnInit {
  activeTab: 'users' | 'tasks' = 'users'; // Default to 'users' tab
  users: DBUser[] = [];
  tasks: DBTask[] = [];
  newUser: Partial<DBUser> = {};
  newTask: Partial<DBTask> = {};
  selectedUser: DBUser | null = null;
  selectedTask: DBTask | null = null;

  constructor(
    private usersService: UsersApiService,
    private tasksService: TasksApiService,
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.getTasks();
    this.getUsers();
  }

  // Handle logout
  handleLogout(): void {
    this.authService.logout(); // Clear authentication token or session
    this.router.navigate(['/login']); // Redirect to login page
  }

  // Fetch all users
  getUsers(): void {
    this.usersService.getUsers().subscribe({
      next: (data) => (this.users = data),
      error: (err) => console.error('Error fetching users', err),
    });
  }

  // Fetch all tasks
  getTasks(): void {
    this.tasksService.getTasks().subscribe({
      next: (data) => (this.tasks = data),
      error: (err) => console.error('Error fetching tasks', err),
    });
  }

  // Create a new user
  createUser(): void {
    if (this.newUser.userName) {
      this.usersService.addUser(this.newUser).subscribe({
        next: (user) => {
          this.users.push(user);
          this.newUser = {};
          alert('User created successfully!');
        },
        error: (err) => console.error('Error creating user', err),
      });
    } else {
      alert('Please fill in all required fields.');
    }
  }

  // Update an existing user
  updateUser(): void {
    if (this.selectedUser) {
      this.usersService.updateUser(this.selectedUser.id, this.selectedUser).subscribe({
        next: () => {
          this.getUsers();
          this.selectedUser = null;
          alert('User updated successfully!');
        },
        error: (err) => console.error('Error updating user', err),
      });
    } else {
      alert('No user selected for update.');
    }
  }

  // Delete a user
  deleteUser(userId: number): void {
    if (confirm('Are you sure you want to delete this user?')) {
      this.usersService.deleteUser(userId).subscribe({
        next: () => {
          this.users = this.users.filter((u) => u.id !== userId);
          alert('User deleted successfully!');
        },
        error: (err) => console.error('Error deleting user', err),
      });
    }
  }

  // Create a new task
  createTask(): void {
    if (
      this.newTask.title &&
      this.newTask.description &&
      this.newTask.assignedUserId
    ) {
      this.tasksService.addTask(this.newTask).subscribe({
        next: (task) => {
          this.tasks.push(task);
          this.newTask = {};
          alert('Task created successfully!');
        },
        error: (err) => console.error('Error creating task', err),
      });
    } else {
      alert('Please fill in all required fields.');
    }
  }

  // Update an existing task
  updateTask(): void {
    if (this.selectedTask) {
      this.tasksService.updateTask(this.selectedTask).subscribe({
        next: () => {
          this.getTasks();
          this.selectedTask = null;
          alert('Task updated successfully!');
        },
        error: (err) => console.error('Error updating task', err),
      });
    } else {
      alert('No task selected for update.');
    }
  }

  // Delete a task
  deleteTask(taskId: number): void {
    if (confirm('Are you sure you want to delete this task?')) {
      this.tasksService.deleteTask(taskId).subscribe({
        next: () => {
          this.tasks = this.tasks.filter((t) => t.id !== taskId);
          alert('Task deleted successfully!');
        },
        error: (err) => console.error('Error deleting task', err),
      });
    }
  }

  // Get the status color for tasks
  getStatusColor(status: string): string {
    switch (status) {
      case 'pending':
        return 'orange';
      case 'in-progress':
        return 'blue';
      case 'completed':
        return 'green';
      default:
        return 'black';
    }
  }

  // Get username by ID
  getUserName(userId: number): string {
    const user = this.users.find((u) => u.id === userId);
    return user ? user.userName : 'Unknown';
  }

  // Switch between tabs
  setActiveTab(tab: 'users' | 'tasks'): void {
    this.activeTab = tab;
  }
}
