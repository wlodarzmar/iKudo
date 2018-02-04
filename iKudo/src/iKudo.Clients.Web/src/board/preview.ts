import { computedFrom, inject } from 'aurelia-framework';
import { BoardService } from '../services/boardService';
import { KudoService } from '../services/kudoService';
import { Notifier } from '../helpers/Notifier';
import { Kudo } from '../viewmodels/kudo';
import { KudoViewModel } from '../viewmodels/kudoViewModel';
import { I18N } from 'aurelia-i18n';

@inject(BoardService, KudoService, Notifier, I18N)
export class Preview {

    public name: string;
    public id: number;
    public kudos: KudoViewModel[] = [];
    public canAddKudo: boolean = false;

    private boardService: BoardService;
    private kudoService: KudoService;
    private notifier: Notifier;
    private i18n: I18N;

    constructor(boardService: BoardService, kudoService: KudoService, notifier: Notifier, i18n: I18N) {

        this.boardService = boardService;
        this.kudoService = kudoService;
        this.notifier = notifier;
        this.i18n = i18n;
    }

    activate(params: any) {
        this.id = params.id;

        this.kudoService.getKudos(this.id, null)
            .then(kudos => {
                let userId: string = JSON.parse(localStorage.getItem('profile')).user_id;
                this.kudos = kudos.map(x => KudoViewModel.convert(x, userId));
            })
            .catch(() => this.notifier.error(this.i18n.tr('kudo.fetch_error')));

        return this.boardService.getWithUsers(params.id)
            .then((board: any) => {
                this.name = board.name;
                let userId: string = JSON.parse(localStorage.getItem('profile')).user_id;
                this.canAddKudo = !board.isPrivate || board.userBoards.map(x => x.userId).indexOf(userId) != -1;
            })
            .catch(error => this.notifier.error(error));
    }
}