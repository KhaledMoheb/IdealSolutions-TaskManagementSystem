import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { Observable } from 'rxjs';
import { AuthService } from './auth.service'; // Adjust the path to your AuthService

@Injectable({
  providedIn: 'root',
})
export class RoleGuard implements CanActivate {
  constructor(private authService: AuthService, private router: Router) {}

  canActivate(
    next: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<boolean> | Promise<boolean> | boolean {
    const requiredRole = next.data['role']; // Role required for this route
    const userRole = this.authService.getUserRole(); // Current user's role

    if (this.authService.isAuthenticated()) {
      if (requiredRole && requiredRole !== userRole) {
        // If the user doesn't have the correct role, redirect them
        this.router.navigate(['/unauthorized']); // Navigate to an unauthorized page or another route
        return false;
      }
      return true; // Allow access
    } else {
      // Redirect to login if not authenticated
      this.router.navigate(['/login']);
      return false;
    }
  }
}
