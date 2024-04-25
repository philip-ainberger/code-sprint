import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ConfigurationService } from './configuration.service';
import { BehaviorSubject, tap, timer } from 'rxjs';

@Injectable({
    providedIn: 'root',
})
export class AuthService {
    private apiBaseUrl: string;
    private tokenSubject = new BehaviorSubject<string | null>(null);

    constructor(private http: HttpClient, private configService: ConfigurationService) {
        this.apiBaseUrl = this.configService.getApiBaseUrl();
        this.initializeTokenRefresh();
    }

    private refreshToken(): void {
        this.http.get<{ token: string, expiresIn: number }>(
            this.apiBaseUrl + '/api/auth/refresh',
            { withCredentials: true }
        ).subscribe({
            next: (response) => {
                const expiresIn = response.expiresIn * 1000;
                const expirationDate = new Date(new Date().getTime() + expiresIn);
                localStorage.setItem('authToken', response.token);
                localStorage.setItem('tokenExpiration', expirationDate.toISOString());
                this.tokenSubject.next(response.token);
                this.scheduleTokenRefresh(expiresIn);
            },
            error: (error) => console.error('Error refreshing token:', error)
        });
    }

    getToken(): BehaviorSubject<string | null> {
        if (!this.tokenSubject.value) {
            this.tokenSubject.next(localStorage.getItem('authToken'));
        }

        return this.tokenSubject;
    }

    validateToken() {
        console.log("Validating token ...");
        var token = this.getToken();

        this.http.get<{ expiresIn: number }>(
            this.apiBaseUrl + '/api/auth/validate',
            { headers: { "Authorization": `Bearer ${token.value}` } }
        ).subscribe({
            next: (response) => {
                const expiresIn = response.expiresIn * 1000;
                const expirationDate = new Date(new Date().getTime() + expiresIn);
                localStorage.setItem('tokenExpiration', expirationDate.toISOString());
                this.scheduleTokenRefresh(expiresIn);
            },
            error: (error) => {
                console.log("Token invalid! Trying to refrehs token ...");
                this.refreshToken();
            }
        });
    }

    private initializeTokenRefresh() {
        const expirationDate = new Date(localStorage.getItem('tokenExpiration') || 0);
        const now = new Date();
        const initialDelay = expirationDate.getTime() - now.getTime() - 60000; // Refresh 1 minute before expiration
        this.scheduleTokenRefresh(initialDelay);
    }

    private scheduleTokenRefresh(delay: number) {
        timer(delay).pipe(
            tap(() => this.refreshToken())
        ).subscribe();
    }
}