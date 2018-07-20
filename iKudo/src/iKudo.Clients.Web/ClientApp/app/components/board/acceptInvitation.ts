import { inject } from "aurelia-framework";
import { AuthService } from "../../services/authService";
import { EventAggregator } from "aurelia-event-aggregator";
import { AuthenticationChangedEventData } from "../../services/models/authentication-changed-event-data.model";
import { Router } from "aurelia-router";
import { BoardService } from "../../services/boardService";
import { Notifier } from "../../helpers/Notifier";

@inject(AuthService, EventAggregator, Router, BoardService, Notifier)
export class AcceptInvitation {

    isAuthenticated: boolean = false;
    private invitationCode: string;
    private boardId: number;

    constructor(
        private readonly authService: AuthService,
        private readonly eventAggregator: EventAggregator,
        private readonly router: Router,
        private readonly boardService: BoardService,
        private readonly notifier: Notifier
    ) { }

    activate(params: any) {

        this.eventAggregator.subscribe('authenticationChange', async (response: AuthenticationChangedEventData) => {

            this.isAuthenticated = response.isAuthenticated;
        });

        console.log(params);
        this.invitationCode = params.code.split(';')[0];
        this.boardId = params.code.split(';')[1];
        this.authService.handleAuthentication();
    }

    login() {
        //TODO: get url from config
        this.authService.login({ redirectUrl: `http://localhost:49862/boards/acceptInvitation?code=${this.invitationCode};${this.boardId}` });
    }

    async acceptInvitation() {
        

        try {
            await this.boardService.acceptInvitation(this.boardId, this.invitationCode);
            this.router.navigate('boards'); //TODO: redirect to accepted board
        } catch (e) {
            console.log(e);
            this.notifier.error('sth went wrong :(');
        }
    }
}