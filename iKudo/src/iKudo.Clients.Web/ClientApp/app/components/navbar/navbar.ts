﻿import { HttpClient } from 'aurelia-fetch-client';
import { inject, observable } from 'aurelia-framework';
import { Router, RouterConfiguration } from 'aurelia-router';
import { NotificationService } from '../../services/notificationService';
import { I18N, BaseI18N } from 'aurelia-i18n';
import * as moment from 'moment';
import * as $ from 'jquery';
import * as bootstrap from 'bootstrap';
import Auth0Lock from 'auth0-lock';
import { ViewModelBase } from '../../viewmodels/viewModelBase';
import { AuthService } from '../../services/authService';
import { EventAggregator } from "aurelia-event-aggregator";
import { UserService } from "../../services/userService";
import { User } from "../../services/models/user";
import { AuthenticationChangedEventData } from "../../services/models/authentication-changed-event-data.model";
import { bindable } from "aurelia-templating";

@inject(HttpClient, Router, I18N, NotificationService, AuthService, EventAggregator, UserService)
export class Navbar extends ViewModelBase {

    public lock: any;
    @observable()
    public isAuthenticated: boolean = false;
    public loggedUser: string = '';
    public userAvatar: string = '';
    public notificationsNumber: number | null = null;
    public notifications: any[] = [];
    @observable()
    public showNavbar: boolean = true;

    constructor(
        private readonly http: HttpClient,
        private router: Router,
        private readonly i18n: I18N,
        private readonly notificationService: NotificationService,
        private readonly authService: AuthService,
        private readonly eventAggregator: EventAggregator,
        private readonly userService: UserService
    ) {

        super();
    }

    async activate(model: any) {
        
        this.router = model.router;
        this.showNavbar = model.showNavbar;

        this.eventAggregator.subscribe('authenticationChange', async (response: AuthenticationChangedEventData) => {

            this.isAuthenticated = response.isAuthenticated;

            if (response.isAuthenticated) {

                this.setUserProperties(response.user);
                await this.addOrUpdateUser(response.user);
                this.setLoadingNotifications();
            }
            else {
                this.router.navigate('');
            }
        });

        this.isAuthenticated = this.authService.isAuthenticated();
        let user = this.authService.getUser();
        if (this.isAuthenticated && user) {
            this.setUserProperties(user);
            this.setLoadingNotifications();
        }
    }

    private setUserProperties(user: User) {
        this.loggedUser = user.name;
        this.userAvatar = user.userAvatar;
    }

    private async addOrUpdateUser(user: User) {
        try {
            await this.userService.addOrUpdate(user);
        } catch (e) {
            console.log(e.message);
        }
    }

    login() {
        this.authService.login();
    }

    logout() {
        this.authService.logout();
    }

    changeLanguage(language: string) {

        this.i18n
            .setLocale(language)
            .then(() => {
                localStorage.setItem('language', language);
            });
    }

    private setLoadingNotifications() {
        this.loadNotifications();
        let self = this;
        setInterval(function () {
            self.loadNotifications();
        }, 30000);
    }

    private loadNotifications() {

        if (!this.currentUserId) {
            return;
        }

        this.notificationService.getNew(this.currentUserId)
            .then((data: any) => {
                let loadedNotificationIds = this.notifications.map(x => x.id);
                this.notifications = this.notifications.filter((el, idx, arr) => {
                    return loadedNotificationIds.indexOf(el.id) == -1;
                }).concat(data);

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
                `<span class="date">${moment(notifications[i].creationDate).format('M/D/YYYY HH:mm')}</span>`,
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