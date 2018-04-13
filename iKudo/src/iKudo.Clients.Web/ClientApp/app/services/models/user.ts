export class User {

    public id: string;
    public firstName: string;
    public lastName: string;
    public email: string;

    get name(): string {
        return `${this.firstName} ${this.lastName}`;
    }
}