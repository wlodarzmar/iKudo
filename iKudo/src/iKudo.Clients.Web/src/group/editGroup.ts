import { HttpClient, json } from 'aurelia-fetch-client';
import { inject } from 'aurelia-framework';

@inject(HttpClient)
export class EditGroup {

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
    }

    activate(params: any) {
        console.log(params.id, 'id');

        this.http.fetch('api/group/' + params.id, {})
            .then(response => response.json().then(data => {
                console.log(data, 'grupa do edycji');
                this.name = data.name;
                this.description = data.description;
            }))
            .catch(error => error.json().then(e => { console.log(e.error); alert('wystpił błąd podczas pobierania grupy'); }));
    }

    submit() {
        alert('submit todo');
    }
}