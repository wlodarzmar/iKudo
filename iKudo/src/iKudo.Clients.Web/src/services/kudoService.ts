import { User } from '../viewmodels/user';
import { KudoType } from '../viewmodels/kudoType';
import { Api } from './api';
import { Kudo } from '../viewmodels/kudo';
import { HttpClient, json } from 'aurelia-fetch-client';
import { inject } from 'aurelia-framework';
import { ErrorParser } from './errorParser';
import * as Uri from 'urijs';

@inject(HttpClient, ErrorParser)
export class KudoService extends Api {

    private errorParser: ErrorParser;
    constructor(http: HttpClient, errorParser: ErrorParser) {
        super(http);
        this.errorParser = errorParser;
    }

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
                .catch(error => error.json().then(e => reject(this.errorParser.parse(e))));
        });
    }

    public getKudos(boardId: number, userId: string, sent: boolean = null, received: boolean = null) {

        return new Promise<Kudo[]>((resolve, reject) => {

            let url = Uri('api/kudos');
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