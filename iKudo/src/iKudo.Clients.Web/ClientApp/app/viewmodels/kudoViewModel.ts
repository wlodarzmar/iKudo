import { Kudo } from './kudo';
import { User } from '../services/models/user';

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
    public status: KudoStatus
    public isApprovalEnabled: boolean;
    public canRemove: boolean;

    private _sender: string;
    get sender(): string {
        return this.isAnonymous && this.currentUser != this._sender ? 'anonim' : this._sender;
    }
    set sender(sender: string) {
        this._sender = sender;
    }


    public static convert(kudo: Kudo, currentUser: string): KudoViewModel {

        let sender = this.convertUser(kudo.sender);
        let receiver = this.convertUser(kudo.receiver);
        
        let kudoVM = new KudoViewModel(kudo.id, kudo.type.name, kudo.description, kudo.creationDate, sender, receiver, kudo.isAnonymous);
        kudoVM.currentUser = currentUser;
        kudoVM.image = kudo.image;
        kudoVM.status = kudo.status;
        kudoVM.isApprovalEnabled = kudo.isApprovalEnabled;
        kudoVM.canRemove = kudo.canRemove;
        return kudoVM;
    }

    private static convertUser(user: User): string {
        if (user.firstName || user.lastName) {
            return `${user.firstName} ${user.lastName}`;
        }
        else {
            return user.email;
        }
    }
}

export enum KudoStatus {
    New = 1,
    Accepted = 2,
    Rejected = 3
}