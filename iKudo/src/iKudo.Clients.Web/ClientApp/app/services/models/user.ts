export class User {

    public id: string;
    public firstName: string;
    public lastName: string;
    public email: string;
    public userAvatar: string;

    get name(): string {
        return `${this.firstName} ${this.lastName}`;
    }
}