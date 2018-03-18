import { inject } from 'aurelia-framework';
import { Notifier } from '../helpers/Notifier';
import { BoardService } from '../../services/boardService';
import { JoinRequestRow } from '../../viewmodels/joinRequestRow';
import { I18N } from 'aurelia-i18n';
import { ViewModelBase } from '../../viewmodels/viewModelBase';

@inject(Notifier, BoardService, I18N)
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

    private notifier: Notifier;
    private boardService: BoardService;
    private i18n: I18N;

    constructor(notifier: Notifier, boardService: BoardService, i18n: I18N) {

        super();
        this.notifier = notifier;
        this.boardService = boardService;
        this.i18n = i18n;
    }

    canActivate(params: any) {

        return new Promise((resolve, reject) => {

            this.boardService.get(params.id)
                .then((board: any) => {

                    //TODO: pobiera się cała tablica, może warto byłoby pobierać tylko creatorId?
                    let userProfile = JSON.parse(localStorage.getItem('profile') || "");
                    let can = this.userId == board.creatorId;
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

                this.id = board.id;
                this.name = board.name;
                this.description = board.description;
                this.creationDate = board.creationDate;
                this.modificationDate = board.modificationDate;
                this.isPrivate = board.isPrivate;
                //TODO: dane usera są brane z aktualnie załadowanego profilu, powinno być pobierane z auth0 ale że dostęp do tej formatki ma tylko właściciel tablicy to tak narazie może zostać
                let userProfile = JSON.parse(localStorage.getItem('profile') || "");
                this.owner = userProfile.name;
                this.ownerEmail = userProfile.email;
            })
            .catch(error => this.notifier.error(this.i18n.tr('boards.fetch_error')));

        this.boardService.getJoinRequestsForBoard(params.id)
            .then((joins: any) => this.parseJoins(joins))
            .catch(() => this.notifier.error(this.i18n.tr('boards.joins_fetch_error')));
    }

    private parseJoins(joins: any) {
        for (let i in joins) {
            let item = joins[i];
            let join = new JoinRequestRow(item.id, item.candidateId, '', item.creationDate);
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
}