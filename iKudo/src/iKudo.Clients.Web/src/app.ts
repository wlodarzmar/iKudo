import { inject } from 'aurelia-framework';
import { HttpClient } from 'aurelia-fetch-client';
import { Router, RouterConfiguration } from 'aurelia-router';

@inject(HttpClient, Router)
export class App {
    public router: Router;
    public http: HttpClient;
    //public secretThing: string;
    //public userName: string;

    // TODO: przenieœæ do konfiguracji
    //lock = new Auth0Lock('DV1nyLKG9TnY8hlHCYXsyv3VgJlqHS1V', 'ikudotest.auth0.com');
    //isAuthenticated = false;

    constructor(http) {
        this.http = http;
        //this.userName = 'USSSEEERRRRRR!!!!!!';
        this.http.configure(config => {
            config.withDefaults({
                mode: 'cors'
            })
                // TODO: przenieœæ do konfiguracji
                .withBaseUrl('http://localhost:49862/');
        });

        var self = this;
        //this.lock.on("authenticated", (authResult) => {
        //    self.lock.getProfile(authResult.idToken, (error, profile) => {
        //        if (error) {
        //            // Handle error
        //            return;
        //        }
        //        console.log(authResult, 'auth result');
        //        console.log(profile, 'profile');

        //        localStorage.setItem('id_token', authResult.idToken);
        //        localStorage.setItem('profile', JSON.stringify(profile));
        //        self.isAuthenticated = true;
        //        self.userName = profile.name;
        //        self.lock.hide();
        //    });
        //});
    }

    //login() {
    //    this.lock.show();
    //}

    //logout() {
    //    localStorage.removeItem('profile');
    //    localStorage.removeItem('id_token');
    //    //this.isAuthenticated = false;
    //}

    getSecretThing() {
       
        this.http.fetch('/api/Test', {
            headers: {
                'Authorization': 'Bearer ' + localStorage.getItem('id_token')

            }
        })
            .then(data => { console.log(data);})
            //.then(response => response.json())
            .catch(x => console.log('b³¹d', x));
    }

    public configureRouter(config: RouterConfiguration, router: Router) {
        config.title = 'iKudo';
        config.map([
            { route: ['', 'welcome'], name: 'welcome', moduleId: 'welcome', nav: true, title: 'Welcome' },
            //{ route: 'users', name: 'users', moduleId: 'users', nav: true, title: 'Github Users' },
            { route: 'child-router', name: 'child-router', moduleId: 'child-router', nav: true, title: 'Child Router' },
            { route: 'test', name: 'test', moduleId: 'test', nav: true, title: 'Test' }
        ]);

        this.router = router;
    }


}
