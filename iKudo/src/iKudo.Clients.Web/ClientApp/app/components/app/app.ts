import { Aurelia, PLATFORM, inject } from 'aurelia-framework';
import { Router, RouterConfiguration } from 'aurelia-router';
import { Api } from '../../services/api';
import { I18N } from 'aurelia-i18n';

@inject(Router, Api, I18N)
export class App {

    showNavbar: boolean = true;

    constructor(
        public readonly router: Router,
        protected readonly api: Api,
        private readonly i18n: I18N
    ) {

        this.setLanguageFromStorage();
    }

    configureRouter(config: RouterConfiguration, router: Router) {
        config.title = 'iKudo.Clients.Web';
        config.options.pushState = true;
        config.map([
            { route: ['/', 'dashboard'], name: 'dashboard', moduleId: PLATFORM.moduleName('../dashboard/dashboard'), nav: true, title: 'menu.dashboard' },
            { route: 'kudo/mykudo', name: 'mykudo', moduleId: PLATFORM.moduleName('../kudo/mykudo'), nav: true, title: 'menu.my_kudo' },
            { route: 'boards/add', name: 'addboard', moduleId: PLATFORM.moduleName('../board/addBoard') },
            { route: 'boards', name: 'boards', moduleId: PLATFORM.moduleName('../board/boards'), nav: true, title: 'menu.boards' },
            { route: 'boards/:id/edit', name: 'editBoard', moduleId: PLATFORM.moduleName('../board/editBoard') },
            { route: 'boards/:id/details', name: 'boardDetails', moduleId: PLATFORM.moduleName('../board/boardDetails') },
            { route: 'boards/:id', name: 'boardPreview', moduleId: PLATFORM.moduleName('../board/preview') },
            { route: 'boards/:id/kudos/add', name: 'addKudo', moduleId: PLATFORM.moduleName('../kudo/addKudo') },
            { route: 'login', name: 'login', moduleId: PLATFORM.moduleName('./login/login.component') },

            { route: 'boards/acceptInvitation', name: 'acceptInvitation', moduleId: PLATFORM.moduleName('../board/acceptInvitation') }
        ]);

        config.fallbackRoute('dashboard'); //TODO: login page instead
    }

    private setLanguageFromStorage() {
        let lang = localStorage.getItem('language');
        if (lang) {
            this.i18n.setLocale(lang);
        }
    }
}
