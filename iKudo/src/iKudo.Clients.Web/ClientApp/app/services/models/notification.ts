import { User } from "./user";
import { Board } from "./board";

export class Notification {
    id: number;
    type: number;
    typeName: string;
    sender: User;
    receiver: User;
    board: Board;
    creationDate: Date;
    readDate: Date;
    isRead: boolean;

    title: string;
    message: string;

    constructor(values: any) {
        Object.assign(this, values);
        this.sender = new User(values.sender);
        this.receiver = new User(values.receiver);
    }
}