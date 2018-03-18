import { KudoType } from './kudoType';

export class Kudo {

    public type: KudoType;
    public receiverId: string;
    public description: string;
    public isAnonymous: boolean;
    public senderId: string;
    public boardId: number;
    public date: Date;
    public image: any;

    constructor(boardId: number, type: KudoType, receiverId: string, senderId: string, description: string) {
        this.type = type;
        this.receiverId = receiverId;
        this.senderId = senderId;
        this.description = description;
        this.boardId = boardId;
    }
}