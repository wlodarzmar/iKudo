import * as auth0 from 'auth0-js';
import { EventAggregator } from 'aurelia-event-aggregator';
import { Router } from 'aurelia-router';
import { inject } from 'aurelia-framework';
import { AuthenticationChangedEventData } from "./models/authentication-changed-event-data.model";
import { User } from "./models/user";
import { AureliaConfiguration } from 'aurelia-configuration';
import { AuthLoginOptions } from "./models/auth-login-options.model";
import { IConfigurationService, ConfigurationService } from './configuration.service';

@inject(Router, EventAggregator, AureliaConfiguration, ConfigurationService)
export class AuthService {

    private readonly authChangeEventName: string = 'authenticationChange';
    private auth0: auth0.WebAuth;

    constructor(
        private readonly router: Router,
        private readonly eventAggregator: EventAggregator,
        private readonly configuration: AureliaConfiguration,
        private readonly configurationService: IConfigurationService
    ) {

        let config = configurationService.getConfiguration();
        
        this.auth0 = new auth0.WebAuth({
            //domain: "ikudotest.auth0.com", //config.auth0Config.domain,
            domain: config.auth0Config.domain,
            clientID: config.auth0Config.clientId,
            redirectUri: config.auth0Config.returnUrl,
            audience: config.auth0Config.audience,
            responseType: 'token id_token',
            scope: 'openid profile email'
        });
    }

    handleAuthentication(redirectRoute?: string) {

        this.auth0.parseHash((err: any, authResult: any) => {

            if (authResult && authResult.accessToken && authResult.idToken) {

                let self = this;
                this.auth0.client.userInfo(authResult.accessToken, (err: any, user: any) => {

                    this.setSession(authResult.accessToken || '', authResult.idToken || '', authResult.expiresIn || 0);
                    let authChangeEventData = this.getEventData(authResult, user);
                    localStorage.setItem('userProfile', JSON.stringify(authChangeEventData.user));
                    this.eventAggregator.publish(self.authChangeEventName, authChangeEventData);

                    if (redirectRoute) {
                        this.router.navigate(redirectRoute);
                    }
                });
            } else if (err) {
                console.log(err);
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
            isAuthenticated: this.isAuthenticated,
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

    login(options?: AuthLoginOptions) {

        if (options) {

            let config = this.configurationService.getConfiguration();

            this.auth0 = new auth0.WebAuth({
                domain: config.auth0Config.domain,
                clientID: config.auth0Config.clientId,
                redirectUri: options.redirectUrl,
                audience: config.auth0Config.audience,
                responseType: 'token id_token',
                scope: 'openid profile email'
            });
        }

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

    get isAuthenticated(): boolean {
        let expiresAt = JSON.parse(localStorage.getItem('expiresAt') || '{}');
        return new Date().getTime() < expiresAt;
    }
}

//TODO: update TS to min 2.4
//enum CookieNames {
//    AccessToken = 'accessToken',
//    UserProfile = 'userProfile',
//    ExpiresAt = 'expiresAt'
//}