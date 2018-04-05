﻿import Auth0Lock from 'auth0-lock';
import { EventAggregator } from 'aurelia-event-aggregator'
import { Router } from 'aurelia-router';
import { inject } from 'aurelia-framework';

@inject(Router, EventAggregator)
export class AuthService {

    private readonly authChangeEventName: string = 'authenticationChange';
    private lock: any;

    constructor(
        private readonly router: Router,
        private readonly eventAggregator: EventAggregator
    ) {
        this.lock = new Auth0Lock('DV1nyLKG9TnY8hlHCYXsyv3VgJlqHS1V', 'ikudotest.auth0.com', {
            auth: {
                audience: 'https://apiikudotest'
            }
        });

        this.lock.on("authenticated", (authResult: any) => {

            this.lock.getProfile(authResult.accessToken, (error: any, profile: any) => {
                if (error) {
                    return;
                }

                this.setSession(authResult.accessToken, authResult.expiresIn, profile);
                this.eventAggregator.publish(this.authChangeEventName, { isAuthenticated: this.isAuthenticated(), userName: profile.name, userAvatar: profile.picture });
                this.lock.hide();
            });
        });
    }

    private setSession(token: string, expiresIn: number, profile: any) {

        localStorage.setItem('accessToken', token);
        localStorage.setItem('profile', JSON.stringify(profile));

        let expiresAt = JSON.stringify(this.expiresInToExpiresAt(expiresIn));
        localStorage.setItem('expiresAt', expiresAt);

        setTimeout(() => {
            this.eventAggregator.publish(this.authChangeEventName, { isAuthenticated: false });
            this.removeSession();
        }, expiresIn * 1000);
    }

    private expiresInToExpiresAt(expiresIn: number): number {
        return expiresIn * 1000 + new Date().getTime();
    }

    login() {
        this.lock.show();
    }

    logout() {

        this.eventAggregator.publish(this.authChangeEventName, { isAuthenticated: false });
        this.removeSession();
    }

    private removeSession() {
        localStorage.removeItem('accessToken');
        localStorage.removeItem('profile');
        localStorage.removeItem('expiresAt');
    }

    isAuthenticated() {
        let expiresAt = JSON.parse(localStorage.getItem('expiresAt') || '{}');
        return new Date().getTime() < expiresAt;
    }
}