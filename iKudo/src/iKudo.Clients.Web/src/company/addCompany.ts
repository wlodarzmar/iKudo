import { HttpClient, json } from 'aurelia-fetch-client';
import { inject } from 'aurelia-framework';

@inject(HttpClient)
export class AddCompany {

    public name: string;
    public description: string;

    private http: HttpClient;

    constructor(http: HttpClient) {
        this.http = http;
        http.configure(config => {
            config.useStandardConfiguration();
            config.withBaseUrl('http://localhost:49862/');
            config.withDefaults({
                credentials: 'same-origin',
                headers: {
                    'X-Requested-With': 'Fetch'
                }
            });
        });
        this.name = 'test name';
        this.description = 'test desc';
    }

    submit() {
        //let addCompanyUrl = 'api/company';
        //let requestBody = {
        //    method: 'post',
        //    body: json({
        //        creatorId: localStorage.getItem('id_token'), name: this.name, description: this.description
        //    })
        //};
        //this.http.fetch(addCompanyUrl, requestAnimationFrame).then(response => console.log(response));

        let getUrl = 'api/company/2';
        this.http.fetch(getUrl, {}).then(response => response.json()).then(result => {
            console.log(result);
        });

    }

    private addCompany(name: string, creatorId: string, description: string) {

    }
}