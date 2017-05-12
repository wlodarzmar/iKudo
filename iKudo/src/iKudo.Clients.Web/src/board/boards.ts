import { HttpClient, json } from 'aurelia-fetch-client';
import { inject } from 'aurelia-framework';
import { BoardRow } from '../viewmodels/boardRow';
import { Notifier } from '../helpers/Notifier';
import { BoardService } from '../services/boardService';

@inject(HttpClient, Notifier, BoardService)
export class Boards {

    public boards: BoardRow[] = [];
    private http: HttpClient;
    private notifier: Notifier;
    private boardService: BoardService;

    constructor(http: HttpClient, notifier: Notifier, boardService: BoardService) {

        // TODO: remove from here
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

        this.boardService = boardService;
        this.http = http;
        this.notifier = notifier;
    }

    activate() {

        return this.http.fetch('api/board', {})
            .then(response => response.json())
            .then(data => {
                console.log(data, 'boards');
                this.toBoardsRow(data);
            })
            .catch(error => { console.log(error, 'error'); error.json().then(e => this.notifier.error(e.error)); });
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
            .then(data => {
                console.log(data);
                this.removeBoard(id);
                this.notifier.info('Usunięto tablicę');
            })
            .catch(error => {
                console.log(error);
                return error.json().then(e => this.notifier.error(e.error));
            });
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

    joinBoard(id: number) {

        this.boardService.join(id)
            .then(() => {
                this.notifier.info("Wysłano prośbę o dołączenie do administratora tablicy");
            })
            .catch(error => error.json()
                .then(e => {
                    console.log(e.error); this.notifier.error(e.error);
                })
            );
    }
}