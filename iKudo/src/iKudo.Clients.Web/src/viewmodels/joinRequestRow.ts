export class JoinRequestRow {

    public id: number;
    public candidate: string;
    public date: Date;
    public email: string;

    constructor(id: number, candidate: string, email: string, date: Date) {

        this.id = id;
        this.candidate = candidate;
        this.email = email;
        this.date = date;
    }
}