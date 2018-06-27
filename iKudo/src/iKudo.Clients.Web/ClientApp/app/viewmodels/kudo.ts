import { KudoType } from './kudoType';
import { User } from "../services/models/user";

export class Kudo {

    public id: number;
    public type: KudoType;
    public receiverId: string; // redundant
    public receiver: User;
    public description: string;
    public isAnonymous: boolean;
    public senderId: string; //redundant
    public sender: User;
    public senderName: string;
    public boardId: number;
    public creationDate: Date;
    public image: any;
    public status: number;
    public isApprovalEnabled: boolean;

    constructor(boardId: number, type: KudoType, receiverId: string, senderId: string, description: string) {
        this.type = type;
        this.receiverId = receiverId;
        this.senderId = senderId;
        this.description = description;
        this.boardId = boardId;
    }
}