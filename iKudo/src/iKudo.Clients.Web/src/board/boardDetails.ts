import { HttpClient, json } from 'aurelia-fetch-client';
import { inject } from 'aurelia-framework';

@inject(HttpClient)
export class BoardDetails {

    public name: string;
    public description: string;
    public owner: string;
    public ownerEmail: string;
    public creationDate: Date;
    public modificationDate: Date;

    private http: HttpClient;

    constructor(http: HttpClient) {

        http.configure(config => {
            config.useStandardConfiguration();
            config.withBaseUrl('http://localhost:49862/');
            config.withDefaults({
                headers: {
                    'Authorization': 'Bearer ' + localStorage.getItem('id_token')
                }
            });
        });

        this.http = http;
    }

    activate(params: any) {
        console.log(params, 'PARAMS');
        this.http.fetch('api/board/' + params.id, {})
            .then(response => response.json().then(data => {

                console.log(data);
                this.name = data.name;
                this.description = data.description;
                this.creationDate = data.creationDate;
                this.modificationDate = data.modificationDate;

                //TODO: dane usera są brane z aktualnie załadowanego profilu, powinno być pobierane z auth0 ale że dostęp do tej formatki ma tylko właściciel tablicy to tak narazie może zostać
                let userProfile = JSON.parse(localStorage.getItem('profile'));
                this.owner = userProfile.name;
                this.ownerEmail = userProfile.email;

            }))
            .catch(error => error.json().then(e => { console.log(e); alert('wystąpił błąd podczas pobierania tablicy'); }));
    }

    canActivate(params: any) {

        console.log(params, 'can activate');
        
        return new Promise((resolve, reject) => {

            this.http.fetch('api/board/' + params.id, {})
                .then(response => response.json().then(data => {
                    
                    //TODO: pobiera się cała tablica, może warto byłoby pobierać tylko creatorId?
                    let userProfile = JSON.parse(localStorage.getItem('profile'));
                    let can = userProfile.user_id == data.creatorId;
                    console.log(can, 'CAN?');
                    resolve(can);
                }))
                .catch(error => error.json().then(e => { console.log(e); resolve(false); }));
        });
    }
}