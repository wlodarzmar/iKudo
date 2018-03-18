import { JoinStatus } from './boardRow';

export class UserJoin {

    public boardId: number;
    public status: JoinStatus;

    constructor(boardId: number, status: JoinStatus) {
        this.boardId = boardId;
        this.status = status;
    } 
}