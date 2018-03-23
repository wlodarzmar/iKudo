import { computedFrom, inject } from 'aurelia-framework';
import { BoardService } from '../../services/boardService';
import { KudoService } from '../../services/kudoService';
import { Notifier } from '../helpers/Notifier';
import { Kudo } from '../../viewmodels/kudo';
import { KudoViewModel } from '../../viewmodels/kudoViewModel';
import { I18N } from 'aurelia-i18n';
import { ViewModelBase } from '../../viewmodels/viewModelBase';

@inject(BoardService, KudoService, Notifier, I18N)
export class Preview extends ViewModelBase {

    public name: string;
    public id: number;
    public kudos: KudoViewModel[] = [];
    public canAddKudo: boolean = false;

    private boardService: BoardService;
    private kudoService: KudoService;
    private notifier: Notifier;
    private i18n: I18N;

    constructor(boardService: BoardService, kudoService: KudoService, notifier: Notifier, i18n: I18N) {

        super();
        this.boardService = boardService;
        this.kudoService = kudoService;
        this.notifier = notifier;
        this.i18n = i18n;
    }

    activate(params: any) {
        this.id = params.id;

        this.kudoService.getKudos(this.id, null)
            .then((kudos: Kudo[]) => {
                this.kudos = kudos.map((x: Kudo) => KudoViewModel.convert(x, this.currentUserId || ''));
            })
            .catch(() => this.notifier.error(this.i18n.tr('kudo.fetch_error')));

        return this.boardService.getWithUsers(params.id)
            .then((board: any) => {
                this.name = board.name;
                this.canAddKudo = !board.isPrivate || board.userBoards.map((x: any) => x.userId).indexOf(this.currentUserId) != -1;
            })
            .catch(error => this.notifier.error(error));
    }
}