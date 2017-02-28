import { HttpClient, json } from 'aurelia-fetch-client';
import { inject } from 'aurelia-framework';

@inject(HttpClient)
export class Boards {

    public boards: any;
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

        http.fetch('api/board', {})
            .then(response => response.json())
            .then(data => { this.boards = data; console.log(data, 'boards'); })
            .catch(error => { console.log(error, 'error'); error.json().then(e=> alert(e.error)); });
    }

    delete(id: number) {
        let body = {
            method: 'DELETE',
        };

        this.http.fetch('api/board/' + id, body)
            .then(data => { console.log(data); this.removeBoard(id); alert('Usunięto grupe'); })
            .catch(error => { console.log(error); return error.json().then(e => alert(e.error)); });
    }

    private removeBoard(id: number) {

        for (let board of this.boards) {
            if (board.id == id) {
                let idx = this.boards.indexOf(board);
                if (idx != -1) {
                    this.boards.splice(idx, 1);
                }
                break;
            }
        }
    }

    edit(id: number) {
        alert(id);
    }
}