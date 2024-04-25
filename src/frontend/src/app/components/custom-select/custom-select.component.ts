import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-custom-select',
  standalone: true,
  imports: [],
  templateUrl: './custom-select.component.html'
})
export class CustomSelectComponent {
  @Input() id: string = "";
  @Input() label: string = "";
}
