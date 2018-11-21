export class User {

    public id: string;
    public firstName: string;
    public lastName: string;
    public email: string;
    public userAvatar: string;

    get name(): string | undefined {
        if (this.firstName != undefined || this.lastName != undefined) {
            return `${this.firstName} ${this.lastName}`;
        }
    }
}