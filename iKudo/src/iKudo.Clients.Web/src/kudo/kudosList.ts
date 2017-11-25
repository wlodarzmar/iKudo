import { computedFrom, inject, observable } from 'aurelia-framework';
import { KudoViewModel } from '../viewmodels/kudoViewModel';
let Masonry = require('masonry-layout');

export class KudosList {

    @observable
    public kudos: KudoViewModel[] = [];
    
    private kudosChanged(newValue: KudoViewModel[], oldValue: KudoViewModel): void {
        let self = this;
        setTimeout(function () {
            self.initGrid();
        }, 1);
    }

    activate(kudos) {
        this.kudos = kudos;
    }

    attached() {
        this.initGrid();
    }

    private initGrid() {
        var elem = document.querySelector('.grid');
        let msnry = new Masonry('.grid', {
            itemSelector: '.grid-item',
            percentPosition: true
        });
    }
}