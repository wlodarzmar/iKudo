﻿import { inject } from 'aurelia-framework';
import { Notifier } from '../../helpers/Notifier';
import { BoardService } from '../../services/boardService';
import { JoinRequestRow } from '../../viewmodels/joinRequestRow';
import { I18N } from 'aurelia-i18n';
import { ViewModelBase } from '../../viewmodels/viewModelBase';
import { AuthService } from "../../services/authService";
import { User } from "../../services/models/user";
import { observable } from "aurelia-binding";
import { InputsHelper } from '../../helpers/inputsHelper';
import { ValidationRules, ValidationController } from "aurelia-validation";
import { NewInstance } from "aurelia-dependency-injection";

@inject(Notifier, BoardService, I18N, AuthService, InputsHelper, NewInstance.of(ValidationController))
export class BoardDetails extends ViewModelBase {

    public id: number;
    public name: string;
    public description: string;
    public owner: string;
    public ownerEmail: string;
    public creationDate: Date;
    public modificationDate: Date;
    public joinRequests: JoinRequestRow[] = [];
    @observable()
    public isPrivate: boolean;
    @observable()
    public kudoAcceptanceEnabled: boolean;
    @observable()
    public kudoAcceptanceAll: boolean;
    public inviteEmail: string;
    public userEmailsToInvite: string[] = [];
    public isSendingInvitations: boolean = false;

    constructor(
        private readonly notifier: Notifier,
        private readonly boardService: BoardService,
        private readonly i18n: I18N,
        private readonly authService: AuthService,
        private readonly inputsHelper: InputsHelper,
        private readonly validationController: ValidationController
    ) {

        super();
    }

    canActivate(params: any) {

        return new Promise((resolve, reject) => {

            this.boardService.find(params.id)
                .then((board: any) => {

                    //TODO: pobiera się cała tablica, może warto byłoby pobierać tylko creatorId?
                    let user = this.authService.getUser() || new User();
                    let can = user.id == board.creatorId;
                    resolve(can);
                })
                .catch(error => {
                    this.notifier.error(this.i18n.tr('boards.fetch_error'));
                    resolve(false);
                })
        });
    }

    activate(params: any) {

        this.initInviteEmailValidation();

        this.boardService.find(params.id)
            .then((board: any) => {
                //TODO: to model
                this.id = board.id;
                this.name = board.name;
                this.description = board.description;
                this.creationDate = board.creationDate;
                this.modificationDate = board.modificationDate;
                this.isPrivate = board.isPrivate;
                this.kudoAcceptanceEnabled = board.acceptanceType != 0;
                this.kudoAcceptanceAll = board.acceptanceType == 1;

                let user = this.authService.getUser() || new User();
                this.owner = user.name;
                this.ownerEmail = user.email;
            })
            .catch(error => this.notifier.error(this.i18n.tr('boards.fetch_error')));

        this.boardService.getJoinRequestsForBoard(params.id)
            .then((joins: any) => this.parseJoins(joins))
            .catch(() => this.notifier.error(this.i18n.tr('boards.joins_fetch_error')));
    }

    private initInviteEmailValidation() {
        ValidationRules.ensure((self: BoardDetails) => self.inviteEmail)
            .email().withMessage(this.i18n.tr('common.invalid_email'))
            .on(this);
    }

    private parseJoins(joins: any) {
        for (let i in joins) {
            let item = joins[i];
            let join = new JoinRequestRow(item.id, item.candidateName, item.candidateEmail, item.creationDate);
            this.joinRequests.push(join);
        }
    }

    acceptJoin(joinId: number) {

        this.boardService.acceptJoin(joinId)
            .then(() => {
                this.notifier.info(this.i18n.tr('boards.join_accepted'));
                this.removeJoinRequest(joinId);
            })
            .catch(() => this.notifier.error(this.i18n.tr('errors.action_error')));
    }

    rejectJoin(joinId: number) {

        this.boardService.rejectJoin(joinId)
            .then(() => {
                this.notifier.info(this.i18n.tr('boards.join_rejected'));
                this.removeJoinRequest(joinId);
            })
            .catch((e) => { console.log(e, 'asd'); this.notifier.error(this.i18n.tr('errors.action_error')); });
    }

    private removeJoinRequest(joinId: number) {

        let idx = this.joinRequests.map((x) => x.id).indexOf(joinId);
        if (idx != -1) {
            this.joinRequests.splice(idx, 1);
        }
    }

    attached() {
        $('[data-toggle="tooltip"]').tooltip({ delay: 1000 });
        this.inputsHelper.Init();
    }

    async isPrivateChanged(newValue: boolean, oldValue: boolean) {
        try {
            await this.boardService.setIsPrivate(this.id, newValue);
            if (newValue) {
                this.kudoAcceptanceAll = true;
            }
        } catch (e) {
            this.notifier.error(this.i18n.tr('errors.action_error'));
        }
    }

    async kudoAcceptanceEnabledChanged() {
        await this.setKudoAcceptanceType();
    }

    async kudoAcceptanceAllChanged(newValue: boolean, oldValue: boolean) {
        await this.setKudoAcceptanceType();
    }

    private async setKudoAcceptanceType() {
        try {

            await this.boardService.setKudoAcceptanceType(this.id, this.getKudoAcceptanceType());
        } catch (e) {
            this.notifier.error(this.i18n.tr('errors.action_error'));
        }
    }

    private getKudoAcceptanceType(): number {

        if (!this.kudoAcceptanceEnabled) {
            return 0
        }

        if (this.kudoAcceptanceAll) {
            return 1;
        }
        else {
            return 2;
        }
    }

    async submitInviteEmail() {
        let validationResult = await this.validationController.validate();
        if (validationResult.valid) {
            if (this.inviteEmail) {
                this.userEmailsToInvite.push(this.inviteEmail);
            }

            this.inviteEmail = '';
        }
    }

    removeUserEmailToInvite(email: string) {
        let idx = this.userEmailsToInvite.indexOf(email);
        this.userEmailsToInvite.splice(idx, 1);
    }

    async sendInvitations() {

        if (this.userEmailsToInvite.length) {
            this.isSendingInvitations = true;
            try {
                await this.boardService.inviteUsers(this.id, this.userEmailsToInvite);
                this.userEmailsToInvite = [];
                this.notifier.success(this.i18n.tr('boards.invitations_sent'));
            } catch (e) {
            }
            finally {
                this.isSendingInvitations = false;
            }
        }
    }
}