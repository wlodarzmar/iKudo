import { inject } from 'aurelia-framework';
import { KudoViewModel, KudoStatus } from '../../viewmodels/kudoViewModel';
import { KudoService } from '../../services/kudoService';
import { EventAggregator } from 'aurelia-event-aggregator';
import { Notifier } from '../../helpers/Notifier';

@inject(KudoService, EventAggregator, Notifier)
export class KudoCardComponent {

    kudo: KudoViewModel;

    constructor(
        private readonly kudoService: KudoService,
        private readonly eventAggregator: EventAggregator,
        private readonly notifier: Notifier
    ) {
    }

    activate(kudo: KudoViewModel) {
        this.kudo = kudo;
    }

    async acceptKudo(id: number) {
        try {
            this.kudoService.accept(id);
            this.eventAggregator.publish('kudoAccepted', id);
            this.kudo.status = KudoStatus.Accepted;
            this.kudo.isApprovalEnabled = false;
        } catch (e) {
            this.notifier.error(e.message);
        }
    }

    async rejectKudo(id: number) {
        try {
            this.kudoService.reject(id);
            this.eventAggregator.publish('kudoRejected', id);

        } catch (e) {
            this.notifier.error(e.message);
        }
    }

    async delete(id: number) {
        try {
            await this.kudoService.delete(id);
            this.eventAggregator.publish('kudoDeleted', id);
        } catch (e) {
            this.notifier.error(e.message);
        }
    }
}