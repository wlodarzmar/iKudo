import { Api } from './api';
import { json } from 'aurelia-fetch-client';

export class NotificationService extends Api {

    public getNew(receiverId: string) {

        return new Promise((resolve, reject) => {

            let url: string = `api/notifications?receiver=${receiverId}&isRead=false&sort=-creationDate`;
            this.http.fetch(url, {})
                .then(response => response.json().then(data => resolve(data)))
                .catch(error => error.json().then((e: any) => reject(e.error)));
        });
    }

    public markAsRead(notification: any) {

        notification.readDate = new Date();
        
        let request = {
            method: 'PUT',
            body: json(notification)
        };

        return new Promise((resolve, reject) => {


            this.http.fetch('api/notifications', request)
                .then(response => resolve(response))
                .catch(error => { error.json().then((e: any) => reject(e.error)); });
        });
    }
}