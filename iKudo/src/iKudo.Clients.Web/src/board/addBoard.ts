import { HttpClient, json } from 'aurelia-fetch-client';
import { inject } from 'aurelia-framework';
import { InputsHelper } from '../inputsHelper';
import { Notifier } from '../helpers/Notifier';

@inject(HttpClient, InputsHelper, Notifier)
export class AddBoard {

    public name: string;
    public description: string;

    private http: HttpClient;
    private notifier: Notifier;
    private inputsHelper;

    constructor(http: HttpClient, InputsHelper, notifier: Notifier) {    
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
        this.notifier = notifier;
        this.inputsHelper = InputsHelper;
    }

    submit() {
        let addCompanyUrl = 'api/board';

        let profile = JSON.parse(localStorage.getItem('profile'));
        let company = {
            Name: this.name,
            Description: this.description
        };

        let requestBody = {
            method: 'POST',
            body: json(company)
        };

        this.http.fetch(addCompanyUrl, requestBody)
            .then(response => response.json())
            .then(data => { console.log(data); this.notifier.info('Dodano tablicę ' + company.Name) })
            .catch(error => { console.log(error, 'error'); error.json().then(e => this.notifier.error(e.error)); });
    }

    attached() {
        this.inputsHelper.Init();
    }
}