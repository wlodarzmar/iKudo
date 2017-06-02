import { HttpClient } from 'aurelia-fetch-client';
import { inject } from 'aurelia-framework';
import { Router, RouterConfiguration } from 'aurelia-router';

@inject(HttpClient, Router)
export class NavBar {

    public router: Router;
    public http: HttpClient;
    public lock = new Auth0Lock('DV1nyLKG9TnY8hlHCYXsyv3VgJlqHS1V', 'ikudotest.auth0.com');
    public isAuthenticated: boolean;
    public loggedUser: string;
    public userAvatar: string;


    constructor(http) {

        this.http = http;

        var self = this;
        this.lock.on("authenticated", (authResult) => {
            self.lock.getProfile(authResult.idToken, (error, profile) => {
                if (error) {
                    return;
                }
                console.log(authResult, 'auth result');
                console.log(profile, 'profile');

                localStorage.setItem('id_token', authResult.idToken);
                localStorage.setItem('profile', JSON.stringify(profile));
                self.isAuthenticated = true;
                this.updateProfileProperties(profile);
                self.lock.hide();
            });
        });
    }

    activate(router) {

        this.router = router;
        this.isAuthenticated = localStorage.getItem('id_token') != undefined;
        this.updateProfileProperties();
    }

    attached() {
        $('body').removeClass('light-blue');
    }

    login() {
        this.lock.show();
    }

    logout() {
        localStorage.removeItem('id_token');
        localStorage.removeItem('profile');
        this.isAuthenticated = false;
        this.router.navigate('/');
    }

    updateProfileProperties(profile: any = null) {

        if (profile == null) {
            profile = JSON.parse(localStorage.getItem('profile'));
            console.log(profile, 'profile from storage');
        }
        if (profile != null) {
            this.loggedUser = profile.name;
            this.userAvatar = profile.picture;
        }
    }
}