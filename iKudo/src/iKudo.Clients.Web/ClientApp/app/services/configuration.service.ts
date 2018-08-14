import { json } from 'aurelia-fetch-client';

export interface IConfigurationService {
    getConfiguration(): AppConfiguration;
}

export class AppConfiguration {
    invitationAcceptUrlFormat: string;
    auth0Config: Auth0Configuration;
}

export class Auth0Configuration {

    returnUrl: string;
    clientId: string;
    domain: string;
    audience: string;
}

declare var appData: any;

export class ConfigurationService implements IConfigurationService {

    getConfiguration() {
        return appData;
    }
}