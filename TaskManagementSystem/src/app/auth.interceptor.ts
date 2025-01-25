import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from './services/auth.service';

export const AuthInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService); // Use dependency injection to get AuthService
  const token = authService.getToken(); // Get the token
  console.log('AuthInterceptor token:', token);
  console.log('AuthInterceptor request:', req);

  if (token) {
    // Clone the request to add the authorization header
    const clonedRequest = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`,
        'Content-Type': 'application/json',
      },
      withCredentials: true
    });
    console.log('clonedRequest', clonedRequest);
    return next(clonedRequest);
  }

  return next(req); // Forward the request if no token is available
};
