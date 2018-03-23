import { ViewModelBase } from "../../viewmodels/viewModelBase";

export class Add extends ViewModelBase {

    constructor() {

        super();
        this.name = 'test name';
    }

    public name: string | undefined = undefined;
    public description: string | undefined = undefined;
}