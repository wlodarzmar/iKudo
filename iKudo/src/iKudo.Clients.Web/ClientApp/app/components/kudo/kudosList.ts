import { computedFrom, inject, observable } from 'aurelia-framework';
import { KudoViewModel, KudoStatus } from '../../viewmodels/kudoViewModel';
import * as Masonry from 'masonry-layout';
import { EventAggregator, Subscription } from 'aurelia-event-aggregator';

@inject(EventAggregator)
export class KudosList {

    constructor(
        private readonly eventAggregator: EventAggregator
    ) { }

    @observable
    public kudos: KudoViewModel[] = [];

    private msnry: Masonry;
    private kudoAcceptedSubscription: Subscription;
    private kudoRejectedSubscription: Subscription;

    private kudosChanged(newValue: KudoViewModel[], oldValue: KudoViewModel[]): void {
        let self = this;
        setTimeout(function () {
            self.initGrid();
        }, 1);
    }

    activate(kudos: KudoViewModel[]) {
        this.kudos = kudos;

        this.kudoAcceptedSubscription = this.eventAggregator.subscribe('kudoAccepted', x => this.onKudoAccepted(x));
        this.kudoRejectedSubscription = this.eventAggregator.subscribe('kudoRejected', x => this.onKudoRejected(x));
    }

    private onKudoAccepted(kudoId: number) {
        this.kudosChanged(this.kudos, this.kudos);
    }

    private onKudoRejected(kudoId: number) {
        let itemToRemove = this.kudos.find(x => x.id == kudoId);
        if (itemToRemove) {
            this.kudos.splice(this.kudos.indexOf(itemToRemove), 1);
            this.kudosChanged(this.kudos, this.kudos);
        }
    }

    attached() {
        this.initGrid();
    }

    private initGrid() {
        var elem = document.querySelector('.grid');
        if (!elem) {
            return;
        }

        this.msnry = new Masonry('.grid', {
            itemSelector: '.grid-item',
            percentPosition: true
        });
    }

    detached() {
        this.kudoAcceptedSubscription.dispose();
        this.kudoRejectedSubscription.dispose();
    }
}