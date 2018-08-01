import { json } from 'aurelia-fetch-client';

export interface IConfigurationService {
    getConfiguration(): AppConfiguration;
}

export class AppConfiguration {
    returnUrl: string;
    invitationAcceptUrlFormat: string;
}

declare var appData: any;

export class ConfigurationService implements IConfigurationService {

    getConfiguration() {
        return appData;
    }
}