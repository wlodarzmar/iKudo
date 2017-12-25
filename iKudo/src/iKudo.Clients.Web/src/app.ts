import { inject } from 'aurelia-framework';
import { Router, RouterConfiguration } from 'aurelia-router';
import { Api } from './services/api';
import { I18N } from 'aurelia-i18n';

@inject(Router, Api, I18N)
export class App {

    public router: Router;
    public api: Api;
    private i18n: I18N;

    constructor(router: Router, api: Api, I18N) {

        this.api = api;
        this.i18n = I18N;

        this.setLanguageFromStorage();
    }

    public configureRouter(config: RouterConfiguration, router: Router) {
        config.title = 'iKudo';
        config.map([

            { route: '/', name: 'dashboard', moduleId: 'dashboard/dashboard', title: 'menu.dashboard' },
            { route: 'dashboard', name: 'dashboard', moduleId: 'dashboard/dashboard', nav: true, title: 'menu.dashboard' },
            { route: 'kudo/mykudo', name: 'mykudo', moduleId: 'kudo/mykudo', nav: true, title: 'menu.my_kudo' },
            { route: 'boards/add', name: 'addboard', moduleId: 'board/addBoard' },
            { route: 'boards', name: 'boards', moduleId: 'board/boards', nav: true, title: 'menu.boards' },
            { route: 'boards/:id/edit', name: 'editBoard', moduleId: 'board/editBoard' },
            { route: 'boards/:id/details', name: 'boardDetails', moduleId: 'board/boardDetails' },
            { route: 'boards/:id', name: 'boardPreview', moduleId: 'board/preview' },
            { route: 'boards/:id/kudos/add', name: 'addKudo', moduleId: 'kudo/addKudo' }
        ]);

        this.router = router;
    }

    private setLanguageFromStorage() {
        let lang = localStorage.getItem('language');
        if (lang) {
            this.i18n.setLocale(lang);
        }
    }
}
