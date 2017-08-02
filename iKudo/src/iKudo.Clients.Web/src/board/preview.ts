import { computedFrom, inject } from 'aurelia-framework';
import { BoardService } from '../services/boardService';
import { KudoService } from '../services/kudoService';
import { Notifier } from '../helpers/Notifier';
import { Kudo } from '../viewmodels/kudo';

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

        this.kudoService.getKudos(this.id)
            .then(kudos => {
                this.kudos = kudos.map(x=>this.toKudoViewModel(x));
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

    private toKudoViewModel(kudo: Kudo): KudoViewModel {

        return new KudoViewModel(kudo.boardId, kudo.type.name, kudo.description, kudo.date, kudo.senderId, kudo.receiverId);
    }

    @computedFrom('kudos')
    get kudos1Column(): KudoViewModel[] {

        let result: KudoViewModel[] = [];
        for (let i = 0; i < this.kudos.length; i++) {
            if (i % 3 == 1) {
                result.push(this.kudos[i]);
            }
        }

        return result;
    }

    @computedFrom('kudos')
    get kudos2Column(): KudoViewModel[] {

        let result: KudoViewModel[] = [];
        for (let i = 0; i < this.kudos.length; i++) {
            if (i % 3 == 2) {
                result.push(this.kudos[i]);
            }
        }

        return result;
    }

    @computedFrom('kudos')
    get kudos3Column(): KudoViewModel[] {

        let result: KudoViewModel[] = [];
        for (let i = 0; i < this.kudos.length; i++) {
            if (i % 3 == 0) {
                result.push(this.kudos[i]);
            }
        }

        return result;
    }
}

export class KudoViewModel {

    constructor(id: number, type: string, text: string, date: Date, sender: string, receiver: string) {

        this.id = id;
        this.type = type;
        this.text = text;
        this.creationDate = date;
        this.sender = sender;
        this.receiver = receiver;
    }
    
    public id: number;
    public type: string;
    public text: string;
    public creationDate: Date;
    public sender: string;
    public receiver: string;
}