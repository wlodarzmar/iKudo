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

    canActivate(params: any) {

        return this.kudoService.getReceivers(params.id, [])
            .then((receivers: User[]) => {
                let userId = JSON.parse(localStorage.getItem('profile')).user_id;
                let currentUserIdx: number = receivers.map(x => x.id).indexOf(userId);
                let can: boolean = currentUserIdx != -1;
                if (can) {
                    this.receivers = receivers.filter(x => x.id != userId);
                }

                return can;
            })
            .catch(() => {
                this.notifier.error('Wystąpił błąd podczas pobierania użytkowników');
                return false;
            });
    }

    activate(params: any) {

        this.kudoService.getKudoTypes()
            .then((types: KudoType[]) => this.types = types)
            .catch(() => this.notifier.error('Wystąpił błąd podczas pobierania danych'));
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