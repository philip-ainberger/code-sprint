import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-custom-badge',
  standalone: true,
  imports: [],
  templateUrl: './custom-badge.component.html'
})
export class CustomBadgeComponent {
  @Input() label: string = "";
}
