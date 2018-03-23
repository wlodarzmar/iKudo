export abstract class ViewModelBase {

    get currentUserId(): string | undefined {
        let profile = localStorage.getItem('profile');
        if (profile) {
            return JSON.parse(localStorage.getItem('profile') || '').sub;
        }

        return undefined;
    }
}