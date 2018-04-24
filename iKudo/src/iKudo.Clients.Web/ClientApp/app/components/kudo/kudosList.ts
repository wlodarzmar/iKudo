import { computedFrom, inject, observable } from 'aurelia-framework';
import { KudoViewModel } from '../../viewmodels/kudoViewModel';
import * as Masonry from 'masonry-layout';

export class KudosList {

    @observable
    public kudos: KudoViewModel[] = [];

    private kudosChanged(newValue: KudoViewModel[], oldValue: KudoViewModel): void {
        let self = this;
        setTimeout(function () {
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

        new Masonry('.grid', {
            itemSelector: '.grid-item',
            percentPosition: true
        });
    }
}