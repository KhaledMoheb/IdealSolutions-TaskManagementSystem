import { Component } from '@angular/core';
import {
  ReactiveFormsModule,
  FormBuilder,
  FormGroup,
  Validators,
} from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service'; // Adjust the path to your AuthService
import { NgIf } from '@angular/common';

@Component({
  selector: 'app-login-page',
  standalone: true,
  templateUrl: './login-page.component.html',
  styleUrls: ['./login-page.component.scss'],
  imports: [ReactiveFormsModule, NgIf],
})
export class LoginPageComponent {
  loginForm: FormGroup;
  isLoading = false;
  errorMessage: string | null = null;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {
    this.loginForm = this.fb.group({
      Username: ['', [Validators.required]],
      password: ['', [Validators.required, Validators.minLength(6)]],
    });
  }

  onSubmit(): void {
    if (this.loginForm.invalid) {
      return;
    }

    this.isLoading = true;
    this.errorMessage = null;

    const { Username, password } = this.loginForm.value;

    this.authService.login(Username, password).subscribe(
      (response) => {
        this.authService.setToken(response.token);
        this.authService.setUser(response.user);

        const redirectRoute =
          response.user.role === 'Admin'
            ? '/admin-dashboard'
            : '/user-dashboard';
        console.log('redirectRoute', redirectRoute);
        this.router.navigate([redirectRoute]);
      },
      (error) => {
        this.errorMessage = 'Invalid username or password. Please try again.';
        this.isLoading = false;
      }
    );
  }
}
