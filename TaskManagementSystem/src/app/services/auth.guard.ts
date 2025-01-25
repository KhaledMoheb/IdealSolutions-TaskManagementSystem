import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { Observable } from 'rxjs';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root',
})
export class AuthGuard implements CanActivate {
  constructor(private authService: AuthService, private router: Router) {}

  canActivate(
    next: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<boolean> | Promise<boolean> | boolean {
    // Get the user role from the authentication service (assuming you have a method to check if the user is authenticated)
    const userRole = this.authService.getUserRole(); // e.g., 'Admin' or 'User'

    // Check if the user is authenticated and has the correct role for the requested route
    if (this.authService.isAuthenticated()) {
      if (next.data['role'] && next.data['role'] !== userRole) {
        // If the route requires a different role, redirect to a different page (e.g., not authorized page)
        this.router.navigate(['/unauthorized']);
        return false;
      }
      return true;
    } else {
      // Redirect to login if not authenticated
      this.router.navigate(['/login']);
      return false;
    }
  }
}
