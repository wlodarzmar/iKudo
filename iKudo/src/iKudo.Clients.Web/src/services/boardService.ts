import { Api } from './api';
import { JoinStatus } from '../viewmodels/boardRow';
import { UserJoin } from '../viewmodels/userJoin';
import { json } from 'aurelia-fetch-client';
import { inject } from 'aurelia-framework';

export class BoardService extends Api {

    public getAll() {

        return new Promise((resolve, reject) => {

            this.http.fetch('api/boards', {})
                .then(response => response.json().then(data => resolve(data)))
                .catch(error => error.json().then(e => reject(e.error)));
        });
    }

    public get(id: number) {

        return new Promise((resolve, reject) => {

            this.http.fetch('api/boards/' + id, {})
                .then(response => response.json().then(data => resolve(data)))
                .catch(error => error.json().then(e => reject(e.error)));
        });
    }

    public getWithUsers(id: number) {

        return new Promise((resolve, reject) => {

            this.http.fetch(`api/boards/${id}?fields=id,name,userboards`, {})
                .then(response => response.json().then(data => resolve(data)))
                .catch(error => error.json().then(e => reject(e.error)));
        });
    }

    public add(board: any) {

        let requestBody = {
            method: 'POST',
            body: json(board)
        };

        return new Promise((resolve, reject) => {

            this.http.fetch('api/boards', requestBody)
                .then(response => response.json().then(data => resolve(data)))
                .catch(error => error.json().then(e => reject(this.parseError(e))));
        });
    }

    private parseError(error: any): string {
        let errorMessage = '';
        for (let i in error) {
            errorMessage += error[i];
        }

        return errorMessage;
    }

    public edit(board: any) {

        let requestBody = {
            method: 'PUT',
            body: json(board)
        };

        return new Promise((resolve, reject) => {

            this.http.fetch('api/boards', requestBody)
                .then(response => resolve(response))
                .catch(error => { error.json().then(e => reject(e.error)); });
        });
    }

    public delete(id: number) {

        let request = {
            method: 'DELETE'
        };

        return new Promise((resolve, reject) => {

            this.http.fetch('api/boards/' + id, request)
                .then(() => resolve())
                .catch(error => error.json().then(e => reject(e.error)));
        });

    }

    public join(boardId: number) {

        let request = {
            method: 'POST',
            body: json(boardId)
        };

        return new Promise((resolve, reject) => {

            this.http.fetch('api/joins', request)
                .then(response => { response.json().then(data => resolve(data)); })
                .catch(error => error.json().then(e => reject(e.error)));
        });
    }

    public getJoinRequests(userId: string): Promise<UserJoin[]> {

        return new Promise((resolve, reject) => {

            this.http.fetch(`api/joins?candidateId=${userId}`, {})
                .then(response => response.json().then(data => resolve(this.toUserJoins(data, userId))))
                .catch(error => error.json().then(e => reject(e.error)));
        });
    }

    private toUserJoins(data: any, userId: string) {

        let userJoins: UserJoin[] = [];

        for (let i in data) {

            let joinRequest = data[i];
            userJoins.push(new UserJoin(joinRequest.boardId, joinRequest.status));
        }

        return userJoins;
    }

    public getJoinRequestsForBoard(boardId: number) {

        return new Promise((resolve, reject) => {

            let url: string = `api/boards/${boardId}/joins?status=waiting`;
            this.http.fetch(url, {})
                .then(response => response.json().then(data => resolve(data)))
                .catch(error => error.json().then(e => reject(e.error)));
        });
    }

    public acceptJoin(joinId: number) {

        return this.sendDecision(joinId, true);
    }

    public rejectJoin(joinId: number) {

        return this.sendDecision(joinId, false);
    }

    private sendDecision(joinId: number, isAccepted: boolean) {

        return new Promise((resolve, reject) => {

            let request = {
                method: 'POST',
                body: json({ joinRequestId: joinId, isAccepted: isAccepted })
            };
            console.log(request);
            this.http.fetch('api/joins/decision', request)
                .then(response => resolve())
                .catch(error => error.json().then(e => reject(e.error)));
        });
    }
}