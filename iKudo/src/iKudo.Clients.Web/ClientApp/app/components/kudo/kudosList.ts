import { computedFrom, inject, observable } from 'aurelia-framework';
import { KudoViewModel, KudoStatus } from '../../viewmodels/kudoViewModel';
import * as Masonry from 'masonry-layout';
import { KudoService } from '../../services/kudoService';

@inject(KudoService)
export class KudosList {

    constructor(
        private readonly kudoService: KudoService,
    ) { }

    @observable
    public kudos: KudoViewModel[] = [];

    private msnry: Masonry;

    private kudosChanged(newValue: KudoViewModel[], oldValue: KudoViewModel[]): void {
        let self = this;
        setTimeout(function () {
            console.log('kudo changed');
            self.initGrid();
        }, 1);
    }

    activate(kudos: KudoViewModel[]) {
        this.kudos = kudos;
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

    acceptKudo(id: number) {
        try {
            this.kudoService.accept(id);

            let item = this.kudos.find(x => x.id == id);
            if (item) {
                item.status = KudoStatus.Accepted;
                item.isApprovalEnabled = false;
                this.kudosChanged(this.kudos, this.kudos);
            }
        } catch (e) {
            console.log(e);
        }
    }

    rejectKudo(id: number) {
        try {
            this.kudoService.reject(id);

            let itemToRemove = this.kudos.find(x => x.id == id);
            if (itemToRemove) {
                this.kudos.splice(this.kudos.indexOf(itemToRemove), 1);
                this.kudosChanged(this.kudos, this.kudos);
            }
        } catch (e) {
            console.log(e);
        }
    }
}