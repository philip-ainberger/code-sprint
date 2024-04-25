import { Component, OnDestroy, OnInit } from '@angular/core';
import { ThemeService } from '../../services/theme.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-layout',
  templateUrl: './layout.component.html',
})
export class LayoutPage implements OnInit, OnDestroy {
  themeSubscription: Subscription;
  currentTheme: string = "";
  
  currentDate: Date = new Date();
  private timer: any;

  constructor(private themeService: ThemeService) {
    this.themeSubscription = this.themeService.getTheme().subscribe(theme => {
      this.currentTheme = theme;
    });
  }

  ngOnInit(): void {
    this.timer = setInterval(() => {
      this.currentDate = new Date();
    }, 60000);
  }

  ngOnDestroy(): void {
    if (this.timer) {
      clearInterval(this.timer);
    }

    if (this.themeSubscription) {
      this.themeSubscription.unsubscribe();
    }
  }

  toggleTheme(): void {
    const newTheme = this.currentTheme === 'light' ? 'dark' : 'light';
    this.themeService.setTheme(newTheme);
  }
}