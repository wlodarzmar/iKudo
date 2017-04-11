import { HttpClient, json } from 'aurelia-fetch-client';
import { inject } from 'aurelia-framework';
import { BoardRow } from '../viewmodels/boardRow';

@inject(HttpClient)
export class Boards {

    public boards: BoardRow[] = [];
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

    activate() {

        return this.http.fetch('api/board', {})
            .then(response => response.json())
            .then(data => {
                console.log(data, 'boards');
                this.toBoardsRow(data);
            })
            .catch(error => { console.log(error, 'error'); error.json().then(e => alert(e.error)); });
    }

    private toBoardsRow(data: any) {
        for (let i in data) {
            let board = data[i];
            let can: boolean = board.creatorId == JSON.parse(localStorage.getItem('profile')).user_id;
            let boardRow = new BoardRow(board.name, board.description, board.id, can, can, can);
            this.boards.push(boardRow);
        }
    }

    delete(id: number) {
        let body = {
            method: 'DELETE',
        };

        this.http.fetch('api/board/' + id, body)
            .then(data => { console.log(data); this.removeBoard(id); })
            .catch(error => { console.log(error); return error.json().then(e => alert(e.error)); });
    }

    private removeBoard(id: number) {
        let boards = this.boards;
        for (let board of boards) {
            if (board.id == id) {
                let idx = boards.indexOf(board);
                if (idx != -1) {
                    boards.splice(idx, 1);
                }
                break;
            }
        }
    }
}