import { inject } from 'aurelia-framework';
import { Router, RouterConfiguration } from 'aurelia-router';
import { Api } from './services/api';

@inject(Router, Api)
export class App {
    public router: Router;

    public api: Api;

    constructor(router: Router, api: Api) {

        this.api = api;
    }

    public configureRouter(config: RouterConfiguration, router: Router) {
        config.title = 'iKudo';
        config.map([
            { route: ['', 'welcome'], name: 'welcome', moduleId: 'welcome', nav: true, title: 'Welcome' },
            { route: 'boards/add', name: 'addboard', moduleId: 'board/addBoard' },
            { route: 'boards', name: 'boards', moduleId: 'board/boards', nav: true, title: 'Tablice' },
            { route: 'boards/:id/edit', name: 'editBoard', moduleId: 'board/editBoard' },
            { route: 'boards/:id/details', name: 'boardDetails', moduleId: 'board/boardDetails' },
            { route: 'child-router', name: 'child-router', moduleId: 'child-router', nav: true, title: 'Child Router' },
            { route: 'test', name: 'test', moduleId: 'test', nav: true, title: 'Test' }
        ]);

        this.router = router;
    }
}
