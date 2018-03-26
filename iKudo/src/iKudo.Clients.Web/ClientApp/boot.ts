import 'isomorphic-fetch';
import { Aurelia, PLATFORM } from 'aurelia-framework';
import { HttpClient } from 'aurelia-fetch-client';
import { I18N, TCustomAttribute } from 'aurelia-i18n';
import * as Backend from 'i18next-xhr-backend';
import 'bootstrap/dist/css/bootstrap.css';
import 'bootstrap';
import * as $ from 'jquery';
let winObj: any = <any>window
winObj['jQuery'] = $;
winObj['$'] = $;
declare const IS_DEV_BUILD: boolean; 

export function configure(aurelia: Aurelia) {
    aurelia.use.standardConfiguration();

    if (IS_DEV_BUILD) {
        aurelia.use.developmentLogging();
    }

    aurelia.use.plugin(PLATFORM.moduleName('aurelia-validation'));
    aurelia.use.plugin(PLATFORM.moduleName('aurelia-dialog'));
    aurelia.use.plugin(PLATFORM.moduleName('aurelia-animator-css'));

    new HttpClient().configure(config => {
        const baseUrl = document.getElementsByTagName('base')[0].href;
        config.withBaseUrl(baseUrl);
    });

    aurelia.use.plugin(PLATFORM.moduleName('aurelia-i18n'), (instance : any) => {
        let aliases = ['t', 'i18n'];
        TCustomAttribute.configureAliases(aliases);
        instance.i18next.use(Backend);

        return instance.setup({
            backend: {
                loadPath: './locales/{{lng}}/{{ns}}.json', 
            },
            attributes: aliases,
            lng: 'pl',
            fallbackLng: 'pl',
            debug: false
        });
    });

    aurelia.start().then(() => aurelia.setRoot(PLATFORM.moduleName('app/components/app/app')));
}
