import { inject } from 'aurelia-framework';
import { DialogController } from 'aurelia-dialog';

@inject(DialogController)
export class DeleteBoardConfirmation {

    private controller: DialogController;
    private answer : any;
    public boardName: string;

    constructor(controller : any) {
        this.controller = controller;
        this.answer = null;

        controller.settings.centerHorizontalOnly = true;
    }

    activate(model: any) {
        this.boardName = model.boardName;
    }
}