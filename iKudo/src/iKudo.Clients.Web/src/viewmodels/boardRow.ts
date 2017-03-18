export class BoardRow {

    public name: string;
    public description: string;
    public id: number;
    public canEdit: boolean;
    public canDetails: boolean;
    public canDelete: boolean;

    constructor(name: string, description: string, id: number, canEdit: boolean, canDetails: boolean, canDelete: boolean) {

        this.name = name;
        this.description = description;
        this.id = id;
        this.canEdit = canEdit;
        this.canDetails = canDetails;
        this.canDelete = canDelete;
    }
}