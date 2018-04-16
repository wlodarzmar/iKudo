import { User } from "../services/models/user";

export abstract class ViewModelBase {

    //TODO: from authService
    get currentUserId(): string | undefined {
        let profile = localStorage.getItem('userProfile');
        if (profile) {
            let user = JSON.parse(localStorage.getItem('userProfile') || '') as User;
            return user.id;
        }

        return undefined;
    }
}