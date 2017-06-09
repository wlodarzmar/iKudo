import { HttpClient } from 'aurelia-fetch-client';
import { inject, observable } from 'aurelia-framework';
import { Router, RouterConfiguration } from 'aurelia-router';
import { NotificationService } from './services/notificationService';

@inject(HttpClient, Router, NotificationService)
export class NavBar {

    public router: Router;
    public http: HttpClient;
    public notificationService: NotificationService;
    public lock = new Auth0Lock('DV1nyLKG9TnY8hlHCYXsyv3VgJlqHS1V', 'ikudotest.auth0.com');
    @observable
    public isAuthenticated: boolean;
    public loggedUser: string;
    public userAvatar: string;
    public notificationsNumber: number;

    constructor(http, router, notificationService) {

        this.http = http;
        this.notificationService = notificationService;
        var self = this;
        this.lock.on("authenticated", (authResult) => {
            self.lock.getProfile(authResult.idToken, (error, profile) => {
                if (error) {
                    return;
                }
                console.log(authResult, 'auth result');
                console.log(profile, 'profile');

                localStorage.setItem('id_token', authResult.idToken);
                localStorage.setItem('profile', JSON.stringify(profile));
                self.isAuthenticated = true;
                this.updateProfileProperties(profile);
                self.lock.hide();

            });
        });
    }

    activate(router) {

        this.router = router;
        this.isAuthenticated = localStorage.getItem('id_token') != undefined;
        this.updateProfileProperties();
    }

    attached() {
        $('body').removeClass('light-blue');
    }

    login() {
        this.lock.show();
    }

    logout() {
        localStorage.removeItem('id_token');
        localStorage.removeItem('profile');
        this.isAuthenticated = false;
        this.router.navigate('/');
    }

    private isAuthenticatedChanged(newValue: boolean, oldValue: boolean) {
        if (newValue) {
            this.getNotificationCount();
        }
    }

    private updateProfileProperties(profile: any = null) {

        if (profile == null) {
            profile = JSON.parse(localStorage.getItem('profile'));
            console.log(profile, 'profile from storage');
        }
        if (profile != null) {
            this.loggedUser = profile.name;
            this.userAvatar = profile.picture;
        }
    }

    private getNotificationCount() {

        let self = this;
        self.notificationService.count()
            .then((count: number) => {
                if (count > 0) {
                    self.notificationsNumber = count;
                    self.loadNotifications();
                }
            })
            .catch(() => console.log("Błąd podczas pobierania liczby powiadiomień"));
    }

    private loadNotifications() {

        let userId = JSON.parse(localStorage.getItem('profile')).user_id;
        this.notificationService.getLatestOrNew(userId)
            .then(data => {
                this.loadNotificationsToPopover(data);
            })
            .catch(() => console.log("Błąd podczas pobierania powiadomień"));        
    }

    private loadNotificationsToPopover(notifications) {

        let popoverTemplate = [
            '<div class="popover notification-popover-wrapper">',
            '<div class="arrow"></div>',
            '<div class="popover-content">',
            '</div>',
            `<p id="notification-history-link"><a href="#">Historia powiadomień</a></p>`,
            '</div>'].join('');

        let options = {
            template: popoverTemplate,
            content: this.prepareNotificationContent(notifications),
            html: true
        };
        $('[data-toggle="popover"]').popover(options);
    }

    private prepareNotificationContent(notifications) {
        let content = '';
        for (let i in notifications) {
            
            content += [
                `<div class="popover-notification ${notifications[i].isRead ? 'read-notification' : 'unread-notification'}">`,
                `<span class="title">${notifications[i].title}</span>`,
                `<span class="date">${notifications[i].date}</span>`,
                `<p class="message">${notifications[i].message}</p>`,
                `</div>`].join('');
        }

        return content;
    }
}