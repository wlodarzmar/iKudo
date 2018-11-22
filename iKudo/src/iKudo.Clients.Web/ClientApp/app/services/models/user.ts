export class User {

   
    constructor(values: any) {
        Object.assign(this, values); 
    }

    public id: string;
    public firstName: string;
    public lastName: string;
    public email: string;
    public userAvatar: string;

    get name(): string | undefined {
        if (this.firstName != undefined || this.lastName != undefined) {
            return `${this.firstName} ${this.lastName}`;
        }
        else {
            return this.email;
        }
    }
}