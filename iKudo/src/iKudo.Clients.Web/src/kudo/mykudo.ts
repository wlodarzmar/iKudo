import { inject } from 'aurelia-framework';
import { KudoService, MyKudoSearchOptions } from '../services/kudoService';
import { Notifier } from '../helpers/Notifier';
import { Kudo } from '../viewmodels/kudo';
import { KudoViewModel } from '../viewmodels/kudoViewModel';
import { ViewModelBase } from '../viewmodels/viewModelBase';

@inject(KudoService, Notifier)
export class MyKudo extends ViewModelBase {

    constructor(kudoService: KudoService, notifier: Notifier) {

        super();
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

    activate() {

        this.findKudosByCriteria();
    }

    public submit() {

        this.findKudosByCriteria();
    }

    private findKudosByCriteria() {
        this.kudoService.getKudos(null, this.userId, this.selectedKudoType)
            .then(kudos => this.kudos = kudos.map(x => {
                return KudoViewModel.convert(x, this.userId);
            }))
            .catch(() => this.notifier.error('Wystąpił błąd podczas pobierania kudosów'));
    }

    public refreshSearch() {
        this.selectedKudoType = MyKudoSearchOptions.All;
    }    
}