import { HttpClient, json } from 'aurelia-fetch-client';
import { inject } from 'aurelia-framework';

@inject(HttpClient)
export class Groups {

    public groups: any;

    constructor(http: HttpClient) {

        http.configure(config => {
            config.useStandardConfiguration();
            config.withBaseUrl('http://localhost:49862/');
        });

        http.fetch('api/company', {})
            .then(response => response.json())
            .then(data => this.groups = data);
    }
}