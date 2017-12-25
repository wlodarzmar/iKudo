import 'bootstrap';
import { Aurelia } from 'aurelia-framework';
import { I18N, Backend, TCustomAttribute } from 'aurelia-i18n';

export function configure(aurelia: Aurelia) {
  aurelia.use
    .standardConfiguration()
    .developmentLogging();

  aurelia.use.plugin('aurelia-validation');
  aurelia.use.plugin('aurelia-dialog');

  // Uncomment the line below to enable animation.
   aurelia.use.plugin('aurelia-animator-css');

  // Anyone wanting to use HTMLImports to load views, will need to install the following plugin.
  // aurelia.use.plugin('aurelia-html-import-template-loader')

   aurelia.use.plugin('aurelia-i18n', (instance) => {
       let aliases = ['t', 'i18n'];
       // add aliases for 't' attribute
       TCustomAttribute.configureAliases(aliases);

       // register backend plugin
       instance.i18next.use(Backend.with(aurelia.loader));

       // adapt options to your needs (see http://i18next.com/docs/options/)
       // make sure to return the promise of the setup method, in order to guarantee proper loading
       return instance.setup({
           backend: {                                  // <-- configure backend settings
               loadPath: './locales/{{lng}}/{{ns}}.json', // <-- XHR settings for where to get the files from
           },
           attributes: aliases,
           lng: 'pl',
           fallbackLng: 'pl',
           debug: false
       });
   });

  aurelia.start().then(() => aurelia.setRoot());
}
