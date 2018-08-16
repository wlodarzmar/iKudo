import { inject } from 'aurelia-framework';
import { NotificationService } from '../../services/notificationService';
import { Notification } from '../../services/models/notification';
import { Notifier } from '../../helpers/Notifier';

@inject(NotificationService, Notifier)
export class NotificationsComponent {

    notifications: Notification[];
    pageSize = 10;

    constructor(
        private readonly notificationService: NotificationService,
        private readonly notifier: Notifier
    ) { }

    async activate() {
        try {
            let notifications = await this.notificationService.getAll();
            this.notifications = notifications;
            console.log(notifications);
        } catch (e) {
            this.notifier.error(e.message);
        }
    }
}