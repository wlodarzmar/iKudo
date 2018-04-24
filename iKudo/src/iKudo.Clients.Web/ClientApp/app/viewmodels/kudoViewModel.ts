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
    public currentUser: string;
    public image: string;

    private _sender: string;
    get sender(): string {
        return this.isAnonymous && this.currentUser != this._sender ? 'anonim' : this._sender;
    }
    set sender(sender: string) {
        this._sender = sender;
    }

    public static convert(kudo: Kudo, currentUser: string): KudoViewModel {

        let sender = '';
        if (kudo.sender) {
            sender = `${kudo.sender.firstName} ${kudo.sender.lastName}`;
        }
        let receiver = '';
        if (kudo.receiver) {

            receiver = `${kudo.receiver.firstName} ${kudo.receiver.lastName}`;
        }

        let kudoVM = new KudoViewModel(kudo.boardId, kudo.type.name, kudo.description, kudo.date, sender, receiver, kudo.isAnonymous);
        kudoVM.currentUser = currentUser;
        kudoVM.image = kudo.image;
        return kudoVM;
    }
}