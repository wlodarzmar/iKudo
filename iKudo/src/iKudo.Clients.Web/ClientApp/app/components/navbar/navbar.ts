import { HttpClient } from 'aurelia-fetch-client';
import { inject, observable } from 'aurelia-framework';
import { Router, RouterConfiguration } from 'aurelia-router';
import { NotificationService } from '../../services/notificationService';
import { I18N, BaseI18N } from 'aurelia-i18n';
import * as moment from 'moment';
import * as $ from 'jquery';
import * as bootstrap from 'bootstrap';
import Auth0Lock from 'auth0-lock';
import { ViewModelBase } from '../../viewmodels/viewModelBase';

@inject(HttpClient, Router, I18N, NotificationService)
export class Navbar extends ViewModelBase {

    public lock: any;
    @observable
    public isAuthenticated: boolean = false;
    public loggedUser: string = '';
    public userAvatar: string ='';
    public notificationsNumber: number | null = null;
    public notifications: any[] = [];

    constructor(
        private readonly http: HttpClient,
        private router: Router,
        private readonly i18n: I18N,
        private readonly notificationService: NotificationService) {

        super();
        this.lock = new Auth0Lock('DV1nyLKG9TnY8hlHCYXsyv3VgJlqHS1V', 'ikudotest.auth0.com', {
            auth: {
                audience: 'https://apiikudotest'
            }
        });
        this.http = http;
        this.i18n = i18n;
        this.notificationService = notificationService;
        this.authenticate();
    }


    activate(router: Router) {
        this.router = router;
        this.isAuthenticated = localStorage.getItem('accessToken') != undefined;
        this.updateProfileProperties();
    }

    login() {
        this.lock.show();
    }

    logout() {
        localStorage.removeItem('accessToken');
        localStorage.removeItem('profile');
        this.isAuthenticated = false;
        this.router.navigate('/');
    }

    changeLanguage(language: string) {

        this.i18n
            .setLocale(language)
            .then(() => {
                localStorage.setItem('language', language);
            });
    }

    private authenticate() {
        this.lock.on("authenticated", (authResult: any) => {

            localStorage.setItem('accessToken', authResult.accessToken);
            this.lock.getProfile(authResult.accessToken, (error: any, profile: any) => {
                if (error) {
                    return;
                }

                localStorage.setItem('profile', JSON.stringify(profile));
                this.isAuthenticated = true;
                this.updateProfileProperties(profile);
                this.lock.hide();

            });

            this.router.navigate('/');
        });
    }

    private isAuthenticatedChanged(newValue: boolean, oldValue: boolean) {

        let self = this;

        if (newValue) {
            this.loadNotifications();

            setInterval(function () {
                self.loadNotifications();
            }, 30000);
        }
    }

    private updateProfileProperties(profile: any = null) {

        if (profile == null) {
            profile = JSON.parse(localStorage.getItem('profile') || "{}");
        }
        if (profile != null) {
            this.loggedUser = profile.name;
            this.userAvatar = profile.picture;
        }
    }

    private loadNotifications() {

        if (!this.currentUserId) {
            return;
        }

        this.notificationService.getNew(this.currentUserId)
            .then((data: any) => {
                this.notifications = this.notifications.concat(data);

                if (data.length) {
                    this.notificationsNumber = data.length;
                }
                this.loadNotificationsToPopover(this.notifications);
            })
            .catch(() => console.log("Błąd podczas pobierania powiadomień"));
    }

    private loadNotificationsToPopover(notifications: any) {

        let self = this;

        let popoverTemplate = [
            '<div class="popover notification-popover-wrapper">',
            '<div class="arrow"></div>',
            '<div class="popover-content">',
            '</div>',
            `<p id="notification-history-link"><a href="#">Historia powiadomień</a></p>`,
            '</div>'].join('');

        let options = {
            template: popoverTemplate,
            html: true
        };

        $('[data-toggle="popover"]').popover(options)
            .off('hidden.bs.popover')
            .on('hidden.bs.popover', function () { self.onNotificationsHidden(); });

        $('[data-toggle="popover"]').attr('data-content', this.prepareNotificationContent(notifications));
    }

    private prepareNotificationContent(notifications: any[]) {

        notifications.sort(function (a, b) {
            a = new Date(a.dateModified);
            b = new Date(b.dateModified);
            return a > b ? -1 : a < b ? 1 : 0;
        }).reverse();
        let content = ' ';

        for (let i in notifications) {

            content += [
                `<div class="popover-notification ${notifications[i].isRead ? 'read-notification' : 'unread-notification'}">`,
                `<span class="title">${notifications[i].title}</span>`,
                `<span class="date">${moment(notifications[i].creationDate).format('M/D/YYYY hh:mm')}</span>`,
                `<p class="message">${notifications[i].message}</p>`,
                `</div>`].join('');
        }

        return content;
    }

    private onNotificationsHidden() {

        for (let i in this.notifications) {

            let notification = this.notifications[i];
            if (!notification.isRead) {
                this.notificationService.markAsRead(notification);
                notification.isRead = true;
            }
        }
        this.notificationsNumber = null;
        this.loadNotificationsToPopover(this.notifications);
    }
}