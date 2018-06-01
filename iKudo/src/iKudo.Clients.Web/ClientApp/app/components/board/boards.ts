import { inject } from 'aurelia-framework';
import { BoardRow, JoinStatus, BoardSearchType } from '../../viewmodels/boardRow';
import { UserJoin } from '../../viewmodels/userJoin';
import { Notifier } from '../../helpers/Notifier';
import { BoardService } from '../../services/boardService';
import { ViewModelBase } from '../../viewmodels/viewModelBase';
import { DialogService } from 'aurelia-dialog';
import { DeleteBoardConfirmation } from './deleteBoardConfirmation';
import { I18N } from 'aurelia-i18n';

@inject(Notifier, BoardService, DialogService, I18N)
export class Boards extends ViewModelBase {

    public boards: BoardRow[] = [];
    public userJoinRequests: UserJoin[] = [];
    public onlyMine: boolean = false;
    public iAmMember: boolean = false;    

    constructor(
        private readonly notifier: Notifier,
        private readonly boardService: BoardService,
        private readonly dialogService: DialogService,
        private readonly i18n: I18N) {

        super();
    }

    canActivate() {
        return this.currentUserId != undefined;
    }

    activate() {

        this.submit();
    }

    submit() {

        let member: string = '';
        let creator: string = '';

        if (this.onlyMine && this.currentUserId) {
            creator = this.currentUserId;
        }

        if (this.iAmMember && this.currentUserId) {
            member = this.currentUserId;
            creator = '';
        }
        

        let getJoinRequestsPromise = this.boardService.getJoinRequests(this.currentUserId);
        let getBoardsPromise = this.boardService.findAll(creator, member);

        Promise.all([getJoinRequestsPromise, getBoardsPromise])
            .then(results => {
                this.userJoinRequests = results[0] as UserJoin[];
                this.toBoardsRow(results[1]);
            })
            .catch(() => this.notifier.error(this.i18n.tr('boards.fetch_error')));
    }

    private toBoardsRow(data: any) {

        this.userJoinRequests = this.userJoinRequests.reverse();
        this.boards = [];
        for (let i in data) {
            let board = data[i];
            let can: boolean = board.creatorId == this.currentUserId;
            let boardRow = new BoardRow(board.name, board.description, board.id, can, can, can);

            if (board.creatorId != this.currentUserId) {
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

        let name = this.findBoard(id)!.name;
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
        this.boardService.remove(id)
            .then(() => {
                this.removeBoardFromModel(id);
                this.notifier.info(this.i18n.tr('boards.removed'));
            })
            .catch(error => this.notifier.error(error));
    }

    private findBoard(id: number)  {

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

                this.notifier.info(this.i18n.tr('boards.join_request_sended'));
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