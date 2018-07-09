import * as auth0 from 'auth0-js';
import { EventAggregator } from 'aurelia-event-aggregator'
import { Router } from 'aurelia-router';
import { inject } from 'aurelia-framework';
import { AuthenticationChangedEventData } from "./models/authentication-changed-event-data.model";
import { User } from "./models/user";
import { AureliaConfiguration } from 'aurelia-configuration';

@inject(Router, EventAggregator, AureliaConfiguration)
export class AuthService {

    private readonly authChangeEventName: string = 'authenticationChange';
    private lock: Auth0LockStatic;
    private auth0: auth0.WebAuth;

    constructor(
        private readonly router: Router,
        private readonly eventAggregator: EventAggregator,
        private readonly configuration: AureliaConfiguration
    ) {

        this.auth0 = new auth0.WebAuth({
            domain: configuration.get('auth0.domain'),
            clientID: configuration.get('auth0.clientId'),
            redirectUri: 'http://localhost:49862/callback',
            audience: configuration.get('auth0.audience'),
            responseType: 'token id_token',
            scope: 'openid profile email'
        });
    }

    handleAuthentication() {
        this.auth0.parseHash((err, authResult) => {

            

            if (authResult && authResult.accessToken && authResult.idToken) {
                
                let self = this;
                this.auth0.client.userInfo(authResult.accessToken, (err, user) => {

                    this.setSession(authResult.accessToken || '', authResult.idToken || '', authResult.expiresIn || 0);
                    let authChangeEventData = this.getEventData(authResult, user);
                    this.eventAggregator.publish(self.authChangeEventName, authChangeEventData);
                    localStorage.setItem('userProfile', JSON.stringify(authChangeEventData.user));

                    this.router.navigate('boards/add');
                });
            } else if (err) {
                console.log(err);
                alert(`Error: ${err.error}. Check the console for further details.`);
            }
        });
    }

    getUser(): User | null {
        let profileString = localStorage.getItem('userProfile');
        if (profileString) {
            return JSON.parse(profileString || '{}') as User;
        }

        return null;
    }

    private getEventData(authResult: any, profile: any) {

        let data: AuthenticationChangedEventData = {
            isAuthenticated: this.isAuthenticated(),
            user: {
                email: profile.email,
                firstName: profile.given_name,
                lastName: profile.family_name,
                userAvatar: profile.picture,
                id: profile.sub,
                name: `${profile.given_name} ${profile.family_name}`
            }
        };

        return data;
    }

    private setSession(accessToken: string, idToken: string, expiresIn: number) {

        localStorage.setItem('accessToken', accessToken);

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

    login(options?: Auth0LockShowOptions) {
        this.auth0.authorize();
    }

    logout() {

        this.removeSession();
        this.eventAggregator.publish(this.authChangeEventName, { isAuthenticated: false });
    }

    private removeSession() {
        localStorage.removeItem('accessToken');
        localStorage.removeItem('profile');
        localStorage.removeItem('expiresAt');
        localStorage.removeItem('userProfile');
    }

    isAuthenticated() {
        let expiresAt = JSON.parse(localStorage.getItem('expiresAt') || '{}');
        return new Date().getTime() < expiresAt;
    }
}