import { inject } from 'aurelia-framework';
import { BoardRow, JoinStatus, BoardSearchType } from '../viewmodels/boardRow';
import { UserJoin } from '../viewmodels/userJoin';
import { Notifier } from '../helpers/Notifier';
import { BoardService } from '../services/boardService';
import { ViewModelBase } from '../viewmodels/viewModelBase';
import { DialogService } from 'aurelia-dialog';
import { DeleteBoardConfirmation } from './deleteBoardConfirmation';

@inject(Notifier, BoardService, DialogService)
export class Boards extends ViewModelBase {

    public boards: BoardRow[] = [];
    public userJoinRequests: UserJoin[];
    public onlyMine: boolean;
    public iAmMember: boolean;
    private notifier: Notifier;
    private boardService: BoardService;
    private dialogService: DialogService;

    constructor(notifier: Notifier, boardService: BoardService, dialogService: DialogService) {

        super();

        this.boardService = boardService;
        this.notifier = notifier;
        this.dialogService = dialogService;
    }

    canActivate() {
        return localStorage.getItem('profile');
    }

    activate() {

        this.submit();
    }

    submit() {

        let member: string = '';
        let creator: string = '';

        if (this.onlyMine) {
            creator = this.userId;
        }
        if (this.iAmMember) {
            member = this.userId;
            creator = '';
        }

        let getJoinRequestsPromise = this.boardService.getJoinRequests(this.userId);
        let getBoardsPromise = this.boardService.getAll(creator, member);

        Promise.all([getJoinRequestsPromise, getBoardsPromise])
            .then(results => {
                this.userJoinRequests = results[0] as UserJoin[];
                this.toBoardsRow(results[1]);
            })
            .catch(() => this.notifier.error('Wystąpił błąd podczas pobierania tablic'));
    }

    private toBoardsRow(data: any) {

        this.userJoinRequests = this.userJoinRequests.reverse();
        this.boards = [];
        for (let i in data) {
            let board = data[i];
            let userId: string = JSON.parse(localStorage.getItem('profile')).user_id;
            let can: boolean = board.creatorId == userId;
            let boardRow = new BoardRow(board.name, board.description, board.id, can, can, can);

            if (board.creatorId != userId) {
                let idx = this.userJoinRequests.map(x => x.boardId).indexOf(board.id);
                boardRow.joinStatus = idx == -1 ? JoinStatus.None : this.userJoinRequests[idx].status;
            }

            this.boards.push(boardRow);
        }
    }

    refreshSearch() {
        this.onlyMine = false;
        this.iAmMember = false;
    }

    delete(id: number) {

        let name = this.findBoard(id).name;
        this.dialogService.open({ viewModel: DeleteBoardConfirmation, model: { boardName: name } })
            .whenClosed(response => {
                if (!response.wasCancelled) {
                    this.removeBoard(id);
                } else {
                    console.log('cancelled');
                }
            });
    }

    private removeBoard(id: number) {
        this.boardService.delete(id)
            .then(() => {
                this.removeBoardFromModel(id);
                this.notifier.info('Usunięto tablicę');
            })
            .catch(error => this.notifier.error(error));
    }

    private findBoard(id: number): BoardRow {

        for (let board of this.boards) {
            if (board.id == id) {
                let idx = this.boards.indexOf(board);
                if (idx != -1) {
                    return board;
                }

                return null;
            }
        }
    }

    private removeBoardFromModel(id: number) {
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