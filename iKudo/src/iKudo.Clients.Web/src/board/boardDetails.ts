import { HttpClient, json } from 'aurelia-fetch-client';
import { inject } from 'aurelia-framework';
import { Notifier } from '../helpers/Notifier';
import { BoardService } from '../services/boardService';

@inject(HttpClient, Notifier, BoardService)
export class BoardDetails {

    public id: number;
    public name: string;
    public description: string;
    public owner: string;
    public ownerEmail: string;
    public creationDate: Date;
    public modificationDate: Date;

    private http: HttpClient;
    private notifier: Notifier;
    private boardService: BoardService;

    constructor(http: HttpClient, notifier: Notifier, boardService: BoardService) {

        this.notifier = notifier;
        this.boardService = boardService;

        // TODO: remove this and implement new method in boardService instead
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
                this.id = data.id;
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