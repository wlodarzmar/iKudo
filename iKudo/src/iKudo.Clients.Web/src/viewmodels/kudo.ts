import { KudoType } from './kudoType';

export class Kudo {

    public type: KudoType;
    public receiverId: string;
    public description: string;
    public isAnonymous: boolean;
    public senderId: string;
    public boardId: number;

    constructor(boardId: number, type: KudoType, receiverId: string, senderId: string, description: string, isAnonymous: boolean) {
        this.type = type;
        this.receiverId = receiverId;
        this.description = description;
        this.isAnonymous = isAnonymous;
        this.boardId = boardId;
    }
}