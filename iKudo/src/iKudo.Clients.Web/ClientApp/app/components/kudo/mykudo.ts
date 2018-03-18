import { inject } from 'aurelia-framework';
import { KudoService } from '../../services/kudoService';
import { Notifier } from '../helpers/Notifier';
import { Kudo } from '../../viewmodels/kudo';
import { KudoViewModel } from '../../viewmodels/kudoViewModel';
import { ViewModelBase } from '../../viewmodels/viewModelBase';
import { I18N } from 'aurelia-i18n';

@inject(KudoService, Notifier, I18N)
export class MyKudo extends ViewModelBase {

    constructor(kudoService: KudoService, notifier: Notifier, i18n: I18N) {

        super();
        this.kudoService = kudoService;
        this.notifier = notifier;
        this.i18n = i18n;
    }

    public sent: boolean = true;
    public received: boolean = true;
    public kudos: KudoViewModel[] = [];

    private kudoService: KudoService;
    private notifier: Notifier;
    private i18n: I18N;

    activate() {

        return this.findKudosByCriteria();
    }

    public submit() {

        this.findKudosByCriteria();
    }

    private findKudosByCriteria() {
        return this.kudoService.getKudos(null, this.userId, this.sent, this.received)
            .then(kudos => this.kudos = kudos.map(x => {
                return KudoViewModel.convert(x, this.userId);
            }))
            .catch(() => this.notifier.error(this.i18n.tr('kudo.fetch_error')));
    }

    public refreshSearch() {
        this.sent = true;
        this.received = true;
    }
}