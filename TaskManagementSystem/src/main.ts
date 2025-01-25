import { enableProdMode } from '@angular/core';
import { bootstrapApplication } from '@angular/platform-browser';
import { AppComponent } from './app/app.component';
import { provideRouter } from '@angular/router';
import { routes } from './app/app.routes';
import { provideHttpClient, withInterceptors } from '@angular/common/http';

import { environment } from './environments/environment';
import { AuthInterceptor } from './app/auth.interceptor';

if (environment.production) {
  enableProdMode();
}

bootstrapApplication(AppComponent, {
  providers: [
    provideRouter(routes), // Provide routing
    provideHttpClient(withInterceptors([AuthInterceptor])) // Provide HttpClient configuration
  ]
}).catch(err => console.error(err));
