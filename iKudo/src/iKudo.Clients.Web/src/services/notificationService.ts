import { Api } from './api';

export class NotificationService extends Api {

    public count() {

        return new Promise((resolve, reject) => {

            this.http.fetch('api/notifications/total', {})
                .then(response => response.json().then(data => resolve(data)))
                .catch(error => error.json().then(e => reject(e.error)));
        });
    }
}