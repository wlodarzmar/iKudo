export abstract class ViewModelBase {

    get userId(): string  {
        return JSON.parse(localStorage.getItem('profile') || "").sub;
    }
}