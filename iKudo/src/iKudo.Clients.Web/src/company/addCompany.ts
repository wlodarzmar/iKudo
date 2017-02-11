import { HttpClient, json } from 'aurelia-fetch-client';
import { inject } from 'aurelia-framework';

@inject(HttpClient)
export class AddCompany {

    public name: string;
    public description: string;

    private http: HttpClient;

    constructor(http: HttpClient) {
        http.configure(config => {
            config.useStandardConfiguration();
            config.withBaseUrl('http://localhost:49862/');
            config.withDefaults(
                {
                    headers: {
                        'Authorization': 'Bearer ' + localStorage.getItem('id_token')
                    }
                });
        });
        this.http = http;
        this.name = 'test name';
        this.description = 'test desc';
    }

    submit() {
        let addCompanyUrl = 'api/company';

        let company = {
            CreatorId: localStorage.getItem('id_token'),
            Name: this.name,
            Description: this.description
        };

        let requestBody = {
            method: 'POST',
            body: json(company)
        };

        this.http.fetch(addCompanyUrl, requestBody)
            .then(response => response.json())
            .then(data => { console.log(data); alert('dodano grupe') })
            .catch(error => { console.log(error, 'error'); error.json().then(e=>alert(e.error)); });
    }
}