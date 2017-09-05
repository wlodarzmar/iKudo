import { computedFrom, inject } from 'aurelia-framework';
import { BoardService } from '../services/boardService';
import { KudoService } from '../services/kudoService';
import { Notifier } from '../helpers/Notifier';
import { Kudo } from '../viewmodels/kudo';
import { KudoViewModel } from '../viewmodels/kudoViewModel';

@inject(BoardService, KudoService, Notifier)
export class Preview {
    constructor(boardService: BoardService, kudoService: KudoService, notifier: Notifier) {

        this.boardService = boardService;
        this.kudoService = kudoService;
        this.notifier = notifier;
    }

    private boardService: BoardService;
    private kudoService: KudoService;
    private notifier: Notifier;
    public name: string;
    public id: number;
    public kudos: KudoViewModel[] = [];
    public canAddKudo: boolean = false;

    activate(params: any) {
        this.id = params.id;

        this.kudoService.getKudos(this.id, null)
            .then(kudos => {
                let userId: string = JSON.parse(localStorage.getItem('profile')).user_id;
                this.kudos = kudos.map(x => KudoViewModel.convert(x, userId));
            })
            .catch(() => this.notifier.error('Wystąpił błąd podczas pobierania kudosów'));

        return this.boardService.getWithUsers(params.id)
            .then((board: any) => {
                this.name = board.name;
                let userId: string = JSON.parse(localStorage.getItem('profile')).user_id;
                this.canAddKudo = board.userboards.map(x => x.userId).indexOf(userId) != -1;
            })
            .catch(error => this.notifier.error(error));
    }
}