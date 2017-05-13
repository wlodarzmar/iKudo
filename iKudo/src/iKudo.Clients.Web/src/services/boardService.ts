import { Api } from './api';
import { json } from 'aurelia-fetch-client';
import { inject } from 'aurelia-framework';

export class BoardService extends Api {

    public getAll() {

        return this.http.fetch('api/board', {});
    }

    public edit(board: any) {

        let requestBody = {
            method: 'PUT',
            body: json(board)
        };

        return this.http.fetch('api/board', requestBody);
    }

    public delete(id: number) {

        let request = {
            method: 'DELETE',
        };

        return this.http.fetch('api/board/' + id, request);
    }

    public join(boardId: number) {
        let request = {
            method: 'POST',
            body: json(boardId)
        };

        return this.http.fetch('api/joinRequest', request);
    }

    public getJoinRequests(userId: string) {

        return this.http.fetch('api/joinRequest', {});
    }
}