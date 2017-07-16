import { json } from 'aurelia-fetch-client';
import { User } from '../viewmodels/user';
import { KudoType } from '../viewmodels/kudoType';
import { Api } from './api';

export class KudoService extends Api {

    public getKudoTypes() {

        return new Promise((resolve, reject) => {
            this.http.fetch('api/kudos/types')
                .then(response => response.json().then((data: Array<any>) => {

                    resolve(data.map(x => new KudoType(x.id, x.name)));
                }))
                .catch(error => error.json().then(e => reject(e.error)));
        });
    }

    public getReceivers(boardId: number): User[] {

        let users: User[] = [];

        users.push(new User(43, 'some ussserrrr'));
        users.push(new User(56, 'some ussserrrr2'));


        return users;
    }
}
