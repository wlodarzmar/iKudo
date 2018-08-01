export interface IConfigurationService {
    getConfiguration(): AppConfiguration;
}

export class AppConfiguration {
    returnUrl: string
}

declare var appData: any;

export class ConfigurationService implements IConfigurationService {

    getConfiguration() {
        return appData;
    }
}