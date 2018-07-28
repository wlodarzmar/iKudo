import { HttpClient } from 'aurelia-fetch-client';
import { inject, observable } from 'aurelia-framework';
import { Router, RouterConfiguration } from 'aurelia-router';
import { NotificationService } from '../../services/notificationService';
import { I18N, BaseI18N } from 'aurelia-i18n';
import * as moment from 'moment';
import * as $ from 'jquery';
import * as bootstrap from 'bootstrap';
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
    public isNotificationPopoverOpen: boolean = false;

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
        this.authService.handleAuthentication();

        this.isAuthenticated = this.authService.isAuthenticated;
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

    attached() {
        //Note: workaround because od popover strange behaviour
        $(document).click((e) => {
            if ($(e.target).parents('.notification-popover-wrapper, .js-notification-globe').length == 0) {
                this.isNotificationPopoverOpen = false;
            }
        });
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

    toggled(isOpen: boolean) {
        if (!isOpen) {
            this.onNotificationsHidden();
        }
    }

    private setLoadingNotifications() {
        this.loadNotifications();
        let self = this;
        setInterval(function () {
            self.loadNotifications();
        }, 30000);
    }

    private async loadNotifications() {

        if (!this.currentUserId) {
            return;
        }

        try {
            let data = await this.notificationService.getNew(this.currentUserId);
            let loadedNotificationIds = data.map((x: any) => x.id);

            this.notifications = this.notifications.filter((el, idx, arr) => {
                return loadedNotificationIds.indexOf(el.id) == -1;
            }).concat(data);

            this.notifications.sort(function (a, b) {
                a = new Date(a.dateModified);
                b = new Date(b.dateModified);
                return a > b ? -1 : a < b ? 1 : 0;
            }).reverse();
            if (data.length) {
                this.notificationsNumber = data.length;
            }
        } catch (e) {
            console.log(e, "Błąd podczas pobierania powiadomień");
        }
    }

    private onNotificationsHidden() {

        let nots = [];
        for (let i in this.notifications) {

            let notification = this.notifications[i];
            if (!notification.isRead) {
                this.notificationService.markAsRead(notification);
                notification.isRead = true;
            }

            nots.push(notification);
        }
        this.notifications = nots;
        this.notificationsNumber = null;
    }
}