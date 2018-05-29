import { inject } from 'aurelia-framework';
import { DialogController } from 'aurelia-dialog';

@inject(DialogController)
export class DeleteBoardConfirmation {

    private answer : any;
    public boardName: string;

    constructor(
        private readonly controller: any) {
        
        this.answer = null;

        controller.settings.centerHorizontalOnly = true;
    }

    activate(model: any) {
        this.boardName = model.boardName;
    }
}