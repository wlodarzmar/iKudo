import { User } from '../services/models/user';
import { KudoType } from '../viewmodels/kudoType';
import { Api } from './api';
import { Kudo } from '../viewmodels/kudo';
import { HttpClient, json } from 'aurelia-fetch-client';
import { inject } from 'aurelia-framework';
import * as Uri from 'urijs';

@inject(HttpClient)
export class KudoService extends Api {

    constructor(
        http: HttpClient,
    ) {

        super(http);
    }

    public async getKudoTypes(): Promise<KudoType[]> {

        let types = await this.get('api/kudos/types');
        return types.map((x: any) => new KudoType(x.id, x.name));
    }

    public async getReceivers(boardId: number, except: string[]): Promise<any> {

        let uri = new Uri('api/users');
        uri.addSearch('boardId', boardId);
        uri.addSearch('except', except.join(','));

        let data = await this.get(uri.valueOf()) as Array<any>;
        return this.createUsers(data);
    }

    public async add(kudo: Kudo) {

        return await this.post('api/kudos', kudo);
    }

    public async getSentByUser(userId: string): Promise<Kudo[]> {
        let url = Uri('api/kudos');
        url.addSearch('sender', userId);
        url.addSearch('status', "accepted");

        return await this.get(url.valueOf());
    }

    public async getReceivedByUser(userId: string): Promise<Kudo[]> {
        let url = Uri('api/kudos');
        url.addSearch('receiver', userId);
        url.addSearch('status', "accepted");
        url.addSearch('sort', '-creationDate');

        return await this.get(url.valueOf());
    }

    public async getByBoard(boardId: number): Promise<Kudo[]> {
        let url = Uri('api/kudos');
        url.addSearch('boardId', boardId);
        url.addSearch('status', 'New, Accepted');
        url.addSearch('sort', '-creationDate');

        return await this.get(url.valueOf());
    }

    public async getKudos(boardId: number | null, userId: string | null, sent: boolean | null = null, received: boolean | null = null): Promise<Kudo[]> {

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

        return await this.get(url.valueOf());
        //return this.convertToKudos(data);
    }

    //private convertToKudos(data: any[]) {
    //    let kudos: Kudo[] = [];
    //    for (let i in data) {

    //        let item = data[i];
    //        let kudo = new Kudo(item.boardId, item.type, item.receiverId, item.senderId, item.description);
    //        kudo.creationDate = item.creationDate;
    //        kudo.isAnonymous = item.isAnonymous;
    //        kudo.image = item.image;
    //        kudo.receiver = item.receiver;
    //        kudo.sender = item.sender;
    //        kudo.status = item.status;
    //        kudo.id = item.id;

    //        kudos.push(kudo);
    //    }

    //    return kudos;
    //}

    public async accept(id: number) {

        let acceptation = { kudoId: id, isAccepted: true };
        return await this.post('api/kudos/approval', acceptation);
    }

    public async reject(id: number) {

        let rejection = { kudoId: id, isAccepted: false };
        return await this.post('api/kudos/approval', rejection);
    }

    public async delete(id: number) {
        return await this.deleteCall(`api/kudos/${id}`);
    }

    private createUsers(users: any): User[] {

        let result = new Array<User>();
        for (var i = 0; i < users.length; i++) {
            result.push(new User(users[i]));
        }

        return result;
    }
}