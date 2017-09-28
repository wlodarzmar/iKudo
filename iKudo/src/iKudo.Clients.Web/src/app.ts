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

            { route: '/', name: 'dashboard', moduleId: 'dashboard/dashboard', title: 'Pulpit' },
            { route: 'dashboard', name: 'dashboard', moduleId: 'dashboard/dashboard', nav: true, title: 'Pulpit' },
            { route: 'kudo/mykudo', name: 'mykudo', moduleId: 'kudo/mykudo', nav: true, title: 'Moje kudo' },
            { route: 'boards/add', name: 'addboard', moduleId: 'board/addBoard' },
            { route: 'boards', name: 'boards', moduleId: 'board/boards', nav: true, title: 'Tablice' },
            { route: 'boards/:id/edit', name: 'editBoard', moduleId: 'board/editBoard' },
            { route: 'boards/:id/details', name: 'boardDetails', moduleId: 'board/boardDetails' },
            { route: 'boards/:id', name: 'boardPreview', moduleId: 'board/preview' },
            { route: 'boards/:id/kudos/add', name: 'addKudo', moduleId: 'kudo/addKudo' }
        ]);

        this.router = router;
    }
}
