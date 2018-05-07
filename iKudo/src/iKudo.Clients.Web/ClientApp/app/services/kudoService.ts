import { User } from '../services/models/user';
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

    public getKudoTypes(): Promise<KudoType[]> {

        return new Promise((resolve, reject) => {
            this.http.fetch('api/kudos/types')
                .then(response => response.json().then((data: Array<any>) => {

                    resolve(data.map(x => new KudoType(x.id, x.name)));
                }))
                .catch(error => error.json().then((e: any) => reject(e.error)));
        });
    }

    public getReceivers(boardId: number, except: string[]) {

        return new Promise<User[]>((resolve, reject) => {
            this.http.fetch(`api/users?boardId=${boardId}&except=${except.join(',')}`)
                .then(response => response.json().then(data => resolve(data)))
                .catch(error => error.json().then((e: any) => reject(e.error)));
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
                .catch(error => error.json().then((e: any) => reject(this.errorParser.parse(e))));
        });
    }

    public async getSentByUser(userId: string): Promise<Kudo[]> {
        let url = Uri('api/kudos');
        url.addSearch('sender', userId);
        url.addSearch('status', "accepted");

        let response = await this.http.fetch(url.valueOf(), {});
        return response.json();
    }

    public async getReceivedByUser(userId: string): Promise<Kudo[]> {
        let url = Uri('api/kudos');
        url.addSearch('receiver', userId);
        url.addSearch('status', "accepted");

        let response = await this.http.fetch(url.valueOf());
        return response.json();
    }

    public async getByBoard(boardId: number): Promise<Kudo[]> {
        let url = Uri('api/kudos?status=accepted');
        url.addSearch('boardId', boardId);

        let response = await this.http.fetch(url.valueOf());
        return response.json();
    }

    public getKudos(boardId: number | null, userId: string | null, sent: boolean | null = null, received: boolean | null = null) {

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

            this.http.fetch(url.valueOf(), {})
                .then(response => response.json().then(data => {
                    resolve(this.convertToKudos(data));
                }))
                .catch(error => error.json().then((e: any) => { reject(e.error); }));
        });
    }

    private convertToKudos(data: any[]) {
        let kudos: Kudo[] = [];
        for (let i in data) {

            let item = data[i];
            let kudo = new Kudo(item.boardId, item.type, item.receiverId, item.senderId, item.description);
            kudo.date = item.creationDate;
            kudo.isAnonymous = item.isAnonymous;
            kudo.image = item.image;
            kudo.receiver = item.receiver;
            kudo.sender = item.sender;

            kudos.push(kudo);
        }

        return kudos;
    }
}