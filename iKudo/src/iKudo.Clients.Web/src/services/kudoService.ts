﻿import { json } from 'aurelia-fetch-client';
import { User } from '../viewmodels/user';
import { KudoType } from '../viewmodels/kudoType';
import { Api } from './api';
import { Kudo } from '../viewmodels/kudo';
//import * as Uri from "urijs";
//import * as moment from "moment";

export class KudoService extends Api {

    private Uri = require('urijs');

    public getKudoTypes() {

    return new Promise((resolve, reject) => {
        this.http.fetch('api/kudos/types')
            .then(response => response.json().then((data: Array<any>) => {

                resolve(data.map(x => new KudoType(x.id, x.name)));
            }))
            .catch(error => error.json().then(e => reject(e.error)));
    });
}

    public getReceivers(boardId: number, except: string[]) {

    return new Promise((resolve, reject) => {
        this.http.fetch(`api/users?boardId=${boardId}&except=${except.join(',')}`)
            .then(response => response.json().then(data => resolve(data)))
            .catch(error => error.json().then(e => reject(e.error)));
    });
}

    public add(kudo: Kudo) {

    return new Promise((resolve, reject) => {

        let requestBody = {
            method: 'POST',
            body: json(kudo)
        };
        this.http.fetch('api/kudos', requestBody)
            .then(response => response.json().then(data => resolve(data)))
            .catch(error => error.json().then(e => reject(e.error)));
    });
}

    public getKudos(boardId: number, userId: string, sent: boolean = null, received: boolean = null) {

    return new Promise<Kudo[]>((resolve, reject) => {

        let url = this.Uri('api/kudos');
        if (boardId) {
            url.addSearch('boardId', boardId);
        }
        if (sent && received) {
            url.addSearch('user', userId);
        }
        else if (sent) {
            url.addSearch('sender', userId);
        }
        else if (received) {
            url.addSearch('receiver', userId);
        }
        else if (sent == false && received == false) {
            url.addSearch('user', 'none');
        }

        this.http.fetch(url.valueOf(), {})
            .then(response => response.json().then(data => {
                resolve(this.convertToKudos(data));
            }))
            .catch(error => error.json().then(e => reject(e.error)));
    });
}

    private convertToKudos(data: any[]) {

    let kudos: Kudo[] = [];
    for (let i in data) {

        let item = data[i];
        let kudo = new Kudo(item.boardId, item.type, item.receiverId, item.senderId, item.description);
        kudo.date = item.creationDate;
        kudo.isAnonymous = item.isAnonymous
        kudos.push(kudo);
    }

    return kudos;
}
}