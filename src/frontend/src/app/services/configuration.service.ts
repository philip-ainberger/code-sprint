import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, firstValueFrom } from 'rxjs';
import { AppConfig } from '../core/app.config';
import { environment } from '../core/environment';

@Injectable({
    providedIn: 'root',
})
export class ConfigurationService {
    private configUrl = environment.configUrl;
    private configSubject: BehaviorSubject<AppConfig> = new BehaviorSubject<AppConfig>(
        {
            apiBaseUrl: ""
        });

    constructor(private http: HttpClient) { }

    async loadConfig(): Promise<void> {
        const config = await firstValueFrom(this.http.get<AppConfig>(this.configUrl));
        this.configSubject.next(config);
    }

    getConfig(): Observable<AppConfig | null> {
        return this.configSubject.asObservable();
    }

    getApiBaseUrl(): string {
        return this.configSubject.getValue().apiBaseUrl;
    }
}