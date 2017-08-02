import { json } from 'aurelia-fetch-client';
import { User } from '../viewmodels/user';
import { KudoType } from '../viewmodels/kudoType';
import { Api } from './api';
import { Kudo } from '../viewmodels/kudo';

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

    public getKudos(boardId: number) {

        return new Promise<Kudo[]>((resolve, reject) => {

            this.http.fetch(`api/kudos?boardId=${boardId}`, {})
                .then(response => response.json().then(data => {
                    console.log(data);
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
            kudos.push(kudo);
        }

        return kudos;
    }
}
