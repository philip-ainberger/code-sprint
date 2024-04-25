import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-animated-number',
  templateUrl: './animated-number.component.html'
})
export class AnimatedNumberComponent implements OnInit {
  @Input() endValue?: number = 100;
  displayValue: number = 0;

  ngOnInit(): void {
    this.animateValue(0, this.endValue!, 1400);
  }

  animateValue(start: number, end: number, duration: number): void {
    let startTimestamp: number | null = null;
    const step = (timestamp: number) => {
      if (!startTimestamp) startTimestamp = timestamp;
      const progress = Math.min((timestamp - startTimestamp) / duration, 1);
      this.displayValue = Math.floor(progress * (end - start) + start);
      if (progress < 1) {
        window.requestAnimationFrame(step);
      }
    };
    window.requestAnimationFrame(step);
  }
}