import { inject } from 'aurelia-framework';
import { Notifier } from '../helpers/Notifier';
import { BoardService } from '../../services/boardService';
import { JoinRequestRow } from '../../viewmodels/joinRequestRow';
import { I18N } from 'aurelia-i18n';
import { ViewModelBase } from '../../viewmodels/viewModelBase';
import { AuthService } from "../../services/authService";
import { User } from "../../services/models/user";

@inject(Notifier, BoardService, I18N, AuthService)
export class BoardDetails extends ViewModelBase{

    public id: number;
    public name: string;
    public description: string;
    public owner: string;
    public ownerEmail: string;
    public creationDate: Date;
    public modificationDate: Date;
    public joinRequests: JoinRequestRow[] = [];
    public isPrivate: boolean;
    public kudoAcceptanceEnabled: boolean;
    public kudoAcceptanceFromExternalUsersEnabled: boolean;

    constructor(
        private readonly notifier: Notifier,
        private readonly boardService: BoardService,
        private readonly i18n: I18N,
        private readonly authService: AuthService) {

        super();
    }

    canActivate(params: any) {

        return new Promise((resolve, reject) => {

            this.boardService.get(params.id)
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

        this.boardService.get(params.id)
            .then((board: any) => {
                //TODO: to model
                this.id = board.id;
                this.name = board.name;
                this.description = board.description;
                this.creationDate = board.creationDate;
                this.modificationDate = board.modificationDate;
                this.isPrivate = board.isPrivate;
                this.kudoAcceptanceEnabled = board.kudoAcceptanceEnabled;
                this.kudoAcceptanceFromExternalUsersEnabled = board.kudoAcceptanceFromExternalUsersEnabled;

                let user = this.authService.getUser() || new User();
                this.owner = user.name;
                this.ownerEmail = user.email;
            })
            .catch(error => this.notifier.error(this.i18n.tr('boards.fetch_error')));

        this.boardService.getJoinRequestsForBoard(params.id)
            .then((joins: any) => this.parseJoins(joins))
            .catch(() => this.notifier.error(this.i18n.tr('boards.joins_fetch_error')));
    }

    private parseJoins(joins: any) {
        for (let i in joins) {
            let item = joins[i];
            console.log(item);
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
            .catch(() => this.notifier.error(this.i18n.tr('errors.action_error')));
    }

    private removeJoinRequest(joinId: number) {

        let idx = this.joinRequests.map((x) => x.id).indexOf(joinId);
        if (idx != -1) {
            this.joinRequests.splice(idx, 1);
        }
    }

    attached() {
        $('[data-toggle="tooltip"]').tooltip({ delay: 1000 });
    }

    async isPrivateChange() {
        try {
            await this.boardService.setIsPrivate(this.id, !this.isPrivate);
            this.isPrivate = !this.isPrivate;
        } catch (e) {
            this.notifier.error(this.i18n.tr('errors.action_error'));
        }
    }

    async kudoAcceptanceChange() {
        try {
            await this.boardService.setKudoAcceptance(this.id, !this.kudoAcceptanceEnabled);
            this.kudoAcceptanceEnabled= !this.kudoAcceptanceEnabled;
        } catch (e) {
            this.notifier.error(this.i18n.tr('errors.action_error'));
        }
    }

    async kudoAcceptanceFromExternalUsersEnabledChange() {
        try {
            await this.boardService.setExternalUsersKudoAcceptance(this.id, !this.kudoAcceptanceFromExternalUsersEnabled);
            this.kudoAcceptanceFromExternalUsersEnabled = !this.kudoAcceptanceFromExternalUsersEnabled;
        } catch (e) {
            this.notifier.error(this.i18n.tr('errors.action_error'));
        }
    }
}