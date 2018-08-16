import { Api } from './api';
import { json, HttpClient } from 'aurelia-fetch-client';
import { I18N } from "aurelia-i18n";
import { inject } from "aurelia-framework";
import { Notification } from "./models/notification";
import * as Uri from 'urijs';

@inject(HttpClient, I18N)
export class NotificationService extends Api {

    private notificationsUrl: string = 'api/notifications';

    constructor(
        http: HttpClient,
        private readonly i18n: I18N) {
        super(http);
    };

    public async getNew(receiverId: string): Promise<Notification[]> {

        let url = new Uri(this.notificationsUrl);
        url.addSearch('receiver', receiverId);
        url.addSearch('isRead', false);
        url.addSearch('sort', '-creationDate');

        let notifications = await this.get(url.valueOf());
        this.generateMessagesAndTitles(notifications);

        return notifications;
    }

    private generateMessagesAndTitles(notifications: Notification[]) {

        for (let notification of notifications) {
            notification.title = this.i18n.tr(`notifications.${notification.typeName}.title`);
            notification.message = this.i18n.tr(`notifications.${notification.typeName}.message`, notification);
        }
    }

    public async markAsRead(notification: any) {

        notification.readDate = new Date();
        return await this.put('api/notifications', notification);
    }

    public async getAll(): Promise<Notification[]> {
        let url = new Uri(this.notificationsUrl);
        url.addSearch('sort', '-creationDate');
        let notifications = await this.get(url.valueOf());
        this.generateMessagesAndTitles(notifications);
        return notifications;
    }
}

