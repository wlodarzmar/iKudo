import { inject } from 'aurelia-framework';
import { KudoService} from '../services/kudoService';
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
    }

    public sent: boolean = true;
    public received: boolean = true;
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
        this.kudoService.getKudos(null, this.userId, this.sent, this.received)
            .then(kudos => this.kudos = kudos.map(x => {
                return KudoViewModel.convert(x, this.userId);
            }))
            .catch(() => this.notifier.error('Wystąpił błąd podczas pobierania kudosów'));
    }

    public refreshSearch() {
        this.sent = true;
        this.received = true;
    }    
}