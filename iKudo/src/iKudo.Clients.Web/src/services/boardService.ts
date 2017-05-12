import { Api } from './api';
import { json } from 'aurelia-fetch-client';
import { inject } from 'aurelia-framework';

export class BoardService extends Api {
    

    public edit(board: any) {

        let requestBody = {
            method: 'PUT',
            body: json(board)
        };

        return this.http.fetch('api/board', requestBody);
    }

    public join(boardId: number) {
        let request = {
            method: 'POST',
            body: json({ BoardId: boardId })
        };

        return this.http.fetch('api/joinRequest', request);
    }
}