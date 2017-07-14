import { User } from '../viewmodels/user';
import { KudoType } from '../viewmodels/kudoType';

export class KudoService {

    public getKudoTypes(): KudoType[] {

        let types: KudoType[] = [];

        types.push(new KudoType(1, "Good Job!"));
        types.push(new KudoType(2, "Congratulations!"));
        types.push(new KudoType(3, "Thank you!"));

        return types;
    }

    public getReceivers(boardId: number): User[] {

        let users: User[] = [];

        users.push(new User(43, 'some ussserrrr'));
        users.push(new User(56, 'some ussserrrr2'));


        return users;
    }
}
