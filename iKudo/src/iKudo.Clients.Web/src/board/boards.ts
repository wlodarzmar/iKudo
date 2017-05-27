import { inject } from 'aurelia-framework';
import { BoardRow, JoinStatus } from '../viewmodels/boardRow';
import { UserJoin } from '../viewmodels/userJoin';
import { Notifier } from '../helpers/Notifier';
import { BoardService } from '../services/boardService';

@inject(Notifier, BoardService)
export class Boards {

    public boards: BoardRow[] = [];
    public userJoinRequests: UserJoin[];
    private notifier: Notifier;
    private boardService: BoardService;

    constructor(notifier: Notifier, boardService: BoardService) {

        this.boardService = boardService;
        this.notifier = notifier;
    }

    activate() {

        let userId = JSON.parse(localStorage.getItem('profile')).user_id;
        let getJoinRequestsPromise = this.boardService.getJoinRequests(userId);

        let getBoardsPromise = this.boardService.getAll();

        Promise.all([getJoinRequestsPromise, getBoardsPromise]).then(results => {

            this.userJoinRequests = results[0] as UserJoin[];
            this.toBoardsRow(results[1]);
        })
            .catch(() => this.notifier.error('Wystąpił błąd podczas pobierania tablic'));
    }

    private toBoardsRow(data: any) {
        for (let i in data) {
            let board = data[i];
            let userId: string = JSON.parse(localStorage.getItem('profile')).user_id;
            let can: boolean = board.creatorId == userId;
            let boardRow = new BoardRow(board.name, board.description, board.id, can, can, can);

            if (board.creatorId != userId) {
                let idx = this.userJoinRequests.map(x => x.boardId).indexOf(board.id);
                boardRow.joinStatus = idx == -1 ? JoinStatus.CanJoin : this.userJoinRequests[idx].status;
            }

            this.boards.push(boardRow);
        }
    }

    delete(id: number) {

        this.boardService.delete(id)
            .then(() => {
                this.removeBoard(id);
                this.notifier.info('Usunięto tablicę');
            })
            .catch(error => this.notifier.error(error));
    }

    private removeBoard(id: number) {
        let boards = this.boards;
        for (let board of boards) {
            if (board.id == id) {
                let idx = boards.indexOf(board);
                if (idx != -1) {
                    boards.splice(idx, 1);
                }
                break;
            }
        }
    }

    joinBoard(id: number) {

        let joinBtn = $("button[data-join-item-btn='" + id + "']");
        joinBtn.attr('disabled', 'true').addClass('disabled');

        this.boardService.join(id)
            .then((joinRequest) => {

                this.notifier.info("Wysłano prośbę o dołączenie do administratora tablicy");
                for (let i in this.boards) {
                    if (this.boards[i].id == id) {
                        this.boards[i].joinStatus = JoinStatus.Waiting;
                    }
                }
            })
            .catch(error => {
                joinBtn.removeAttr('disabled').removeClass('disabled');
                this.notifier.error(error);
            });
    }
}