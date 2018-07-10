import { computedFrom, inject } from 'aurelia-framework';
import { BoardService } from '../../services/boardService';
import { KudoService } from '../../services/kudoService';
import { Notifier } from '../../helpers/Notifier';
import { Kudo } from '../../viewmodels/kudo';
import { KudoViewModel, KudoStatus } from '../../viewmodels/kudoViewModel';
import { I18N } from 'aurelia-i18n';
import { ViewModelBase } from '../../viewmodels/viewModelBase';
import { AuthService } from '../../services/authService';
import { User } from "../../services/models/user";
import { List } from 'linqts';

@inject(BoardService, KudoService, Notifier, I18N, AuthService)
export class Preview extends ViewModelBase {

    public name: string;
    public id: number;
    public kudos: KudoViewModel[] = [];
    public canAddKudo: boolean = false;

    constructor(
        private readonly boardService: BoardService,
        private readonly kudoService: KudoService,
        private readonly notifier: Notifier,
        private readonly i18n: I18N,
        private readonly authService: AuthService
    ) {
        super();
    }

    activate(params: any) {
        this.id = params.id;
        //TODO to asyn/await

        this.kudoService.getByBoard(this.id)
            .then((kudos: Kudo[]) => {
                let user: User = this.authService.getUser() || new User();
                this.kudos = kudos.map((x: Kudo) => KudoViewModel.convert(x, user.name));
            })
            .catch((e) => {
                this.notifier.error(this.i18n.tr('kudo.fetch_error'));
            });

        return this.boardService.getWithUsers(params.id)
            .then((board: any) => {
                this.name = board.name;
                this.canAddKudo = !board.isPrivate || board.userBoards.map((x: any) => x.userId).indexOf(this.currentUserId) != -1;
            })
            .catch(error => this.notifier.error(error));
    }


}