import { inject } from 'aurelia-framework';
import { InputsHelper } from '../inputsHelper';
import { KudoService } from '../services/kudoService';
import { User } from '../viewmodels/user';
import { KudoType } from '../viewmodels/kudoType';

@inject(InputsHelper, KudoService)
export class AddKudo {

    constructor(inputHelper: InputsHelper, kudoService: KudoService) {
        this.inputHelper = inputHelper;
        this.kudoService = kudoService;
    }

    public selectedType: KudoType;
    public selectedReceiver: User;
    public isAnonymous: boolean = false;
    public text: string;
    public types: KudoType[] = [];
    public receivers: User[] = [];

    private inputHelper: InputsHelper;
    private kudoService: KudoService;

    activate() {

        this.types = this.kudoService.getKudoTypes();

        let boardId = 2;
        this.receivers = this.kudoService.getReceivers(boardId);
    }

    attached() {
        this.inputHelper.Init();
    }

    submit() {

        console.log(this.selectedType, 'sel type');
        console.log(this.selectedReceiver, 'sel receiver');
        console.log(this.isAnonymous, 'is anonyn');
    }
}