import { inject } from 'aurelia-framework';
import { KudoViewModel, KudoStatus } from '../../viewmodels/kudoViewModel';
import { KudoService } from '../../services/kudoService';
import { EventAggregator } from 'aurelia-event-aggregator';

@inject(KudoService, EventAggregator)
export class KudoCardComponent {

    kudo: KudoViewModel;

    constructor(
        private readonly kudoService: KudoService,
        private readonly eventAggregator: EventAggregator
    ) {
    }

    activate(kudo: KudoViewModel) {
        console.log(kudo.isApprovalEnabled, "KUDO");
        this.kudo = kudo;
    }

    acceptKudo(id: number) {
        try {
            this.kudoService.accept(id);
            this.eventAggregator.publish('kudoAccepted', id);
            this.kudo.status = KudoStatus.Accepted;
            this.kudo.isApprovalEnabled = false;
        } catch (e) {
            console.log(e);
        }
    }

    rejectKudo(id: number) {
        try {
            this.kudoService.reject(id);
            this.eventAggregator.publish('kudoRejected', id);

        } catch (e) {
            console.log(e);
        }
    }
}