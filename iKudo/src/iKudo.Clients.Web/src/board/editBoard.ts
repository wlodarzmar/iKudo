import { HttpClient, json } from 'aurelia-fetch-client';
import { inject } from 'aurelia-framework';
import { InputsHelper } from '../inputsHelper';
import { Notifier } from '../helpers/Notifier';
import { BoardService } from '../services/boardService';

@inject(HttpClient, InputsHelper, Notifier, BoardService)
export class EditBoard {

    public name: string;
    public description: string;
    public id: number;
    public creatorId: string;
    public creationDate: Date

    private http: HttpClient;
    private inputsHelper: InputsHelper;
    private notifier: Notifier;
    private boardService: BoardService;

    constructor(http: HttpClient, InputsHelper, notifier: Notifier, boardService: BoardService) {

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
        this.inputsHelper = InputsHelper;
        this.notifier = notifier;
        this.boardService = boardService;
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

    activate(params: any) {

        this.http.fetch('api/board/' + params.id, {})
            .then(response => response.json().then(data => {
                console.log(data, 'grupa do edycji');
                this.name = data.name;
                this.description = data.description;
                this.id = data.id;
                this.creatorId = data.creatorId;
                this.creationDate = data.creationDate;

                setTimeout(() => this.inputsHelper.Init(), 100);
            }))
            .catch(error => error.json()
                .then(e => {
                    console.log(e.error); this.notifier.error('wystpił błąd podczas pobierania grupy');
                }));
    }

    submit() {

        let board = {
            Id: this.id,
            CreatorId: this.creatorId,
            Name: this.name,
            Description: this.description,
            CreationDate: this.creationDate
        };

        this.boardService.edit(board)
            .then(() => {
                this.notifier.info("Zapisano zmiany w tablicy '" + board.Name + "'");
            })
            .catch(error => error.json()
                .then(e => {
                    console.log(e.error); this.notifier.error('Wystpił błąd podczas zapisu');
                })
            );
    }
}