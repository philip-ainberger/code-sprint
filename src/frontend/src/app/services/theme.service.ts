import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ThemeService {
  private currentTheme = new BehaviorSubject<string>('light'); // Default theme

  constructor() {
    const savedTheme = localStorage.getItem('theme') ?? 'light';
    this.currentTheme.next(savedTheme);
    this.updateDocumentClass(savedTheme);
  }

  getCurrentTheme(): string {
    return localStorage.getItem('theme') ?? "light";
  }

  setTheme(theme: string): void {
    this.currentTheme.next(theme);
    localStorage.setItem('theme', theme);
    this.updateDocumentClass(theme);
  }

  getTheme(): BehaviorSubject<string> {
    return this.currentTheme;
  }

  private updateDocumentClass(theme: string): void {
    if (theme === 'dark') {
      document.documentElement.classList.add('dark');
    } else {
      document.documentElement.classList.remove('dark');
    }
  }
}