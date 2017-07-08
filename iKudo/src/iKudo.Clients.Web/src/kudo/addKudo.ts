import { inject } from 'aurelia-framework';
import { InputsHelper } from '../inputsHelper';

@inject(InputsHelper)
export class AddKudo {

    constructor(inputHelper: InputsHelper) {
        this.inputHelper = inputHelper;
    }

    public type: number;
    public userId: string;
    public isAnonymous: boolean;
    public text: string;


    private inputHelper: InputsHelper;

    attached() {
        this.inputHelper.Init();
    }
}