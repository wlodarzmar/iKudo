export class JoinRequestRow {

    public id: string;
    public candidate: string;
    public date: Date;
    public email: string;

    constructor(id: string, candidate: string, email: string, date: Date) {

        this.id = id;
        this.candidate = candidate;
        this.email = email;
        this.date = date;
    }
}