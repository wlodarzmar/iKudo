import { inject } from 'aurelia-framework';
import { Notifier } from '../helpers/Notifier';
import { BoardService } from '../services/boardService';
import { JoinRequestRow } from '../viewmodels/joinRequestRow';

@inject(Notifier, BoardService)
export class BoardDetails {

    public id: number;
    public name: string;
    public description: string;
    public owner: string;
    public ownerEmail: string;
    public creationDate: Date;
    public modificationDate: Date;
    public joinRequests: JoinRequestRow[] = [];

    private notifier: Notifier;
    private boardService: BoardService;

    constructor(notifier: Notifier, boardService: BoardService) {

        this.notifier = notifier;
        this.boardService = boardService;

        let j1: JoinRequestRow = new JoinRequestRow("12321", "Jan Nowak", "asdf@asdf.pl", new Date());
        let j2: JoinRequestRow = new JoinRequestRow("12321", "Janina Kowalska", "asdf@asdf.pl", new Date);
        let j3: JoinRequestRow = new JoinRequestRow("12321", "Marian Paździoch", "asdf@asdf.pl", new Date());

        this.joinRequests.push(j1, j2, j3);
    }

    canActivate(params: any) {

        return new Promise((resolve, reject) => {

            this.boardService.get(params.id)
                .then((board: any) => {

                    //TODO: pobiera się cała tablica, może warto byłoby pobierać tylko creatorId?
                    let userProfile = JSON.parse(localStorage.getItem('profile'));
                    let can = userProfile.user_id == board.creatorId;
                    resolve(can);
                })
                .catch(error => {
                    this.notifier.error('Wystąpił błąd podczas pobierania tablicy');
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
                //TODO: dane usera są brane z aktualnie załadowanego profilu, powinno być pobierane z auth0 ale że dostęp do tej formatki ma tylko właściciel tablicy to tak narazie może zostać
                let userProfile = JSON.parse(localStorage.getItem('profile'));
                this.owner = userProfile.name;
                this.ownerEmail = userProfile.email;
            })
            .catch(error => this.notifier.error('Wystąpił błąd podczas pobierania tablicy'));

        this.boardService.getJoinRequestsForBoard(params.id)
            .then(() => console.log('ok'))
            .catch(() => console.log('Not OK'));
    }

    attached() {
        $('[data-toggle="tooltip"]').tooltip({ delay: 1000 });
    }
}