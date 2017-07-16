import { inject } from 'aurelia-framework';
import { InputsHelper } from '../inputsHelper';
import { KudoService } from '../services/kudoService';
import { User } from '../viewmodels/user';
import { KudoType } from '../viewmodels/kudoType';
import { Notifier } from '../helpers/Notifier';

@inject(InputsHelper, KudoService, Notifier)
export class AddKudo {

    constructor(inputHelper: InputsHelper, kudoService: KudoService, notifier: Notifier) {
        this.inputHelper = inputHelper;
        this.kudoService = kudoService;
        this.notifier = notifier;
    }

    public selectedType: KudoType;
    public selectedReceiver: User;
    public isAnonymous: boolean = false;
    public text: string;
    public types: KudoType[] = [];
    public receivers: User[] = [];

    private inputHelper: InputsHelper;
    private kudoService: KudoService;
    private notifier: Notifier;

    activate(params: any) {

        this.kudoService.getKudoTypes()
            .then((types: KudoType[]) => this.types = types)
            .catch(() => this.notifier.error('Wystąpił błąd podczas pobierania danych'));

        this.kudoService.getReceivers(params.id)
            .then((receivers: User[]) => this.receivers = receivers)
            .catch(() => this.notifier.error('Wystąpił błąd podczas pobierania użytkowników'));
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