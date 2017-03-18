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

    }

    getSecretThing() {

        this.http.fetch('/api/Test', {
            headers: {
                'Authorization': 'Bearer ' + localStorage.getItem('id_token')

            }
        })
            .then(data => { console.log(data); })
            //.then(response => response.json())
            .catch(x => console.log('b³¹d', x));
    }

    public configureRouter(config: RouterConfiguration, router: Router) {
        config.title = 'iKudo';
        config.map([
            { route: ['', 'welcome'], name: 'welcome', moduleId: 'welcome', nav: true, title: 'Welcome' },
            { route: 'boards/add', name: 'addboard', moduleId: 'board/addBoard', nav: true, title: 'Add Board' },
            { route: 'boards', name: 'boards', moduleId: 'board/boards', nav: true, title: 'Tablice' },
            { route: 'boards/:id/edit', name: 'editBoard', moduleId: 'board/editBoard' },
            { route: 'boards/:id/details', name: 'boardDetails', moduleId: 'board/boardDetails' },
            { route: 'child-router', name: 'child-router', moduleId: 'child-router', nav: true, title: 'Child Router' },
            { route: 'test', name: 'test', moduleId: 'test', nav: true, title: 'Test' }
        ]);

        this.router = router;
    }


}
