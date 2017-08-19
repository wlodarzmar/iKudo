import { Kudo } from './kudo';

export class KudoViewModel {

    constructor(id: number, type: string, text: string, date: Date, sender: string, receiver: string, isAnonymous: boolean) {

        this.id = id;
        this.type = type;
        this.text = text;
        this.creationDate = date;
        this.sender = sender;
        this.receiver = receiver;
        this.isAnonymous = isAnonymous;
    }

    public id: number;
    public type: string;
    public text: string;
    public creationDate: Date;
    public receiver: string;
    public isAnonymous: boolean;

    private _sender: string;
    get sender(): string {
        return this.isAnonymous ? 'anonim' : this._sender;
    }
    set sender(sender: string) {
        this._sender = sender;
    }

    public static convert(kudo: Kudo): KudoViewModel {

        return new KudoViewModel(kudo.boardId, kudo.type.name, kudo.description, kudo.date, kudo.senderId, kudo.receiverId, kudo.isAnonymous);
    }
}