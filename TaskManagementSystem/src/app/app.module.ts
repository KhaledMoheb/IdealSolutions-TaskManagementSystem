import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { RouterModule } from '@angular/router';

import { routes } from './app.routes';
import { AppComponent } from './app.component';

import {
  HTTP_INTERCEPTORS,
  provideHttpClient,
  withInterceptorsFromDi,
} from '@angular/common/http';
import { AdminDashboardComponent } from './pages/admin-dashboard/admin-dashboard.component';
import { UserDashboardComponent } from './pages/user-dashboard/user-dashboard.component';
import { LoginPageComponent } from './pages/login-page/login-page.component';
import { AuthInterceptor } from './auth.interceptor';
// import { AuthInterceptor } from './interceptors/auth-interceptor.service';

// Import standalone components
@NgModule({
  declarations: [],
  imports: [
    BrowserModule,
    RouterModule.forRoot(routes),
    AppComponent,
    AdminDashboardComponent,
    UserDashboardComponent,
    LoginPageComponent,
  ],
  providers: [
  ],
  bootstrap: [],
})
export class AppModule {}
