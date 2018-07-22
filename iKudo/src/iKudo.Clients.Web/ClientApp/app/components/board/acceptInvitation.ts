import { inject, Aurelia, PLATFORM } from "aurelia-framework";
import { AuthService } from "../../services/authService";
import { EventAggregator } from "aurelia-event-aggregator";
import { AuthenticationChangedEventData } from "../../services/models/authentication-changed-event-data.model";
import { Router } from "aurelia-router";
import { BoardService } from "../../services/boardService";
import { Notifier } from "../../helpers/Notifier";
import { App } from "../app/app";
import { Parent } from "aurelia-dependency-injection";
import { I18N } from "aurelia-i18n";
import { AureliaConfiguration } from "aurelia-configuration";
import { String } from 'typescript-string-operations';

@inject(AuthService, EventAggregator, Router, BoardService, Notifier, Parent.of(App), I18N, AureliaConfiguration)
export class AcceptInvitation {

    isAuthenticated: boolean = false;
    private invitationCode: string;
    private boardId: number;

    constructor(
        private readonly authService: AuthService,
        private readonly eventAggregator: EventAggregator,
        private readonly router: Router,
        private readonly boardService: BoardService,
        private readonly notifier: Notifier,
        private readonly app: App,
        private readonly i18n: I18N,
        private readonly configuration: AureliaConfiguration
    ) {
    }

    activate(params: any) {

        this.eventAggregator.subscribe('authenticationChange', async (response: AuthenticationChangedEventData) => {

            this.isAuthenticated = response.isAuthenticated;
        });

        this.invitationCode = params.code.split(';')[0];
        this.boardId = params.code.split(';')[1];
        this.authService.handleAuthentication();
    }

    attached() {
        this.app.showNavbar = false;
    }

    detached() {
        this.app.showNavbar = true;
    }

    login() {
        let urlFormat = this.configuration.get('boardInvitationAcceptUrl');
        let url = String.Format(urlFormat, this.invitationCode, this.boardId);
        this.authService.login({ redirectUrl: url });
    }

    async acceptInvitation() {

        try {
            await this.boardService.acceptInvitation(this.boardId, this.invitationCode);
            this.router.navigate(`boards/${this.boardId}`);
        } catch (e) {
            console.log(e);
            this.notifier.error(this.i18n.tr('errors.action_error'));
        }
    }
}