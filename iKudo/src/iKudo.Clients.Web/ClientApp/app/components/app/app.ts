import { Aurelia, PLATFORM, inject } from 'aurelia-framework';
import { Router, RouterConfiguration } from 'aurelia-router';
import { Api } from '../../services/api';
import { I18N } from 'aurelia-i18n';

@inject(Router, Api, I18N)
export class App {

    public router: Router;
    public api: Api;
    private i18n: I18N;

    constructor(router: Router, api: Api, i18n: I18N) {

        this.api = api;
        this.i18n = i18n;

        this.setLanguageFromStorage();
    }

    configureRouter(config: RouterConfiguration, router: Router) {
        config.title = 'iKudo.Clients.Web';
        config.map([
            { route: ['/', 'dashboard'], name: 'dashboard', moduleId: PLATFORM.moduleName('../dashboard/dashboard'), nav: true, title: 'menu.dashboard' },
            { route: 'kudo/mykudo', name: 'mykudo', moduleId: PLATFORM.moduleName('../kudo/mykudo'), nav: true, title: 'menu.my_kudo' },
            { route: 'boards/add', name: 'addboard', moduleId: PLATFORM.moduleName('../board/addBoard') },
            { route: 'boards', name: 'boards', moduleId: PLATFORM.moduleName('../board/boards'), nav: true, title: 'menu.boards' },
            { route: 'boards/:id/edit', name: 'editBoard', moduleId: PLATFORM.moduleName('../board/editBoard') },
            { route: 'boards/:id/details', name: 'boardDetails', moduleId: PLATFORM.moduleName('../board/boardDetails') },
            { route: 'boards/:id', name: 'boardPreview', moduleId: PLATFORM.moduleName('../board/preview') },
            { route: 'boards/:id/kudos/add', name: 'addKudo', moduleId: PLATFORM.moduleName('../kudo/addKudo') }
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
