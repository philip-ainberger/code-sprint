import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class StateService {
    private currentState = new BehaviorSubject<string>('Hello 👋');

    setState(state: string): void {
        this.currentState.next(state);
    }

    getState(): BehaviorSubject<string> {
        return this.currentState;
    }
}