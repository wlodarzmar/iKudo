import { Api } from './api';
import { JoinStatus } from '../viewmodels/boardRow';
import { UserJoin } from '../viewmodels/userJoin';
import { json } from 'aurelia-fetch-client';
import { inject } from 'aurelia-framework';

export class BoardService extends Api {

    public getAll() {

        return new Promise((resolve, reject) => {

            this.http.fetch('api/board', {})
                .then(response => response.json().then(data => resolve(data)))
                .catch(error => error.json().then(e => reject(e.error)));
        });
    }

    public get(id: number) {

        return new Promise((resolve, reject) => {

            this.http.fetch('api/board/' + id, {})
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

            this.http.fetch('api/board', requestBody)
                .then(response => response.json().then(data => resolve(data)))
                .catch(error => error.json().then(e => reject(e.error)));
        });
    }

    public edit(board: any) {

        let requestBody = {
            method: 'PUT',
            body: json(board)
        };

        return new Promise((resolve, reject) => {

            this.http.fetch('api/board', requestBody)
                .then(response => resolve(response))
                .catch(error => { error.json().then(e => reject(e.error)); });
        });
    }

    public delete(id: number) {

        let request = {
            method: 'DELETE',
        };

        return new Promise((resolve, reject) => {

            this.http.fetch('api/board/' + id, request)
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

            this.http.fetch('api/joinRequest', request)
                .then(response => { response.json().then(data => resolve(data)); })
                .catch(error => error.json().then(e => reject(e.error)));
        });
    }

    public getJoinRequests(userId: string) {

        return new Promise((resolve, reject) => {

            this.http.fetch('api/joinRequest', {})
                .then(response => response.json().then(data => resolve(this.toUserJoins(data, userId))))
                .catch(error => error.json().then(e => reject(e.error)));
        });
    }

    private toUserJoins(data: any, userId: string) {

        let userJoins: UserJoin[] = [];

        for (let i in data) {
            console.log(i, 'i');
            let joinRequest = data[i];
            userJoins.push(new UserJoin(joinRequest.boardId, this.getStatus(joinRequest, userId)));
        }

        console.log(userJoins, 'DATA');
        return userJoins;
    }

    private getStatus(joinRequest: any, userId: string) {

        if (joinRequest.isAccepted == true) {
            return JoinStatus.Joined;
        }
        else if (joinRequest.isAccepted == false) {
            return JoinStatus.Waiting;
        }
        else {
            return JoinStatus.CanJoin;
        }

    }
}