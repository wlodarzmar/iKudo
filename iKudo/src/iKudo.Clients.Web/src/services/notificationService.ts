import { Api } from './api';
import { json } from 'aurelia-fetch-client';

export class NotificationService extends Api {

    public getLatestOrNew(receiverId: string) {

        let notifications = [
            { isRead: false, date: '6/6/2017', title: 'Title 1', message: 'Użytkownik user1 wysłał prośbę o dołączenie do tablicy test' },
            { isRead: false, date: '6/6/2017', title: 'title2', message: 'Użytkownik user2 wysłał prośbę o dołączenie do tablicy test' },
            { isRead: true, date: '6/6/2017', title: '', message: 'Użytkownik user3 wysłał prośbę o dołączenie do tablicy test' }
        ];

        return new Promise((resolve, reject) => {

            let url: string = `api/notifications?receiver=${receiverId}&isRead=false`;
            this.http.fetch(url, {})
                .then(response => response.json().then(data => resolve(data)))
                .catch(error => error.json().then(e => reject(e.error)));
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
                .catch(error => { error.json().then(e => reject(e.error)); });
        });

    }
}