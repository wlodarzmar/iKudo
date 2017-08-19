import { inject, computedFrom } from 'aurelia-framework';
import { KudoService, MyKudoSearchOptions } from '../services/kudoService';
import { Notifier } from '../helpers/Notifier';
import { Kudo } from '../viewmodels/kudo';
import { KudoViewModel } from '../viewmodels/kudoViewModel';

@inject(KudoService, Notifier)
export class MyKudo {

    constructor(kudoService: KudoService, notifier: Notifier) {

        this.kudoService = kudoService;
        this.notifier = notifier;
        this.kudoTypes = [MyKudoSearchOptions.All, MyKudoSearchOptions.Received, MyKudoSearchOptions.Sended];
        this.selectedKudoType = MyKudoSearchOptions.All;
    }

    public kudoTypes: MyKudoSearchOptions[];
    public selectedKudoType: MyKudoSearchOptions;
    public kudos: KudoViewModel[] = [];

    private kudoService: KudoService;
    private notifier: Notifier;

    public submit() {

        let userId = JSON.parse(localStorage.getItem('profile')).user_id;
        this.kudoService.getKudos(null, userId, this.selectedKudoType)
            .then(kudos => this.kudos = kudos.map(x => KudoViewModel.convert(x)))
            .catch(() => this.notifier.error('Wystąpił błąd podczas pobierania kudosów'));
    }
    
    public refreshSearch() {
        this.selectedKudoType = MyKudoSearchOptions.All;
    }

    
}