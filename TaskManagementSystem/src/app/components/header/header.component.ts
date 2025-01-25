import { Component, Input, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-header',
  standalone: true,
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss'],
})
export class HeaderComponent {
  @Input() username!: string; // Display the logged-in user's name
  @Output() logout = new EventEmitter<void>(); // Emit logout event

  onLogout() {
    this.logout.emit(); // Trigger logout event
  }
}
