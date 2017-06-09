import { Api } from './api';

export class NotificationService extends Api {

    public count() {

        return new Promise((resolve, reject) => {

            this.http.fetch('api/notifications/total', {})
                .then(response => response.json().then(data => resolve(data)))
                .catch(error => error.json().then(e => reject(e.error)));
        });
    }

    public getLatestOrNew(receiverId: string) {

        let notifications = [
            { isRead: false, date: '6/6/2017', title: 'Title 1', message: 'Użytkownik user1 wysłał prośbę o dołączenie do tablicy test' },
            { isRead: false, date: '6/6/2017', title: 'title2', message: 'Użytkownik user2 wysłał prośbę o dołączenie do tablicy test' },
            { isRead: true, date: '6/6/2017', title: '', message: 'Użytkownik user3 wysłał prośbę o dołączenie do tablicy test' }
        ];

        return new Promise((resolve, reject) => {

            resolve(notifications);
        });
    }
}