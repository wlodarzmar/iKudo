import { computedFrom } from 'aurelia-framework';
import { KudoViewModel } from '../viewmodels/kudoViewModel';

export class KudosList {
    
    public kudos: KudoViewModel[] = [];

    activate(kudos) {
        console.log(kudos, 'params Activate');
        this.kudos = kudos;
    }

    @computedFrom('kudos')
    get kudos1Column(): KudoViewModel[] {

        let result: KudoViewModel[] = [];
        if (this.kudos) {
            for (let i = 0; i < this.kudos.length; i++) {
                if (i % 3 == 1) {
                    result.push(this.kudos[i]);
                }
            }
        }

        return result;
    }

    @computedFrom('kudos')
    get kudos2Column(): KudoViewModel[] {

        let result: KudoViewModel[] = [];
        if (this.kudos) {
            for (let i = 0; i < this.kudos.length; i++) {
                if (i % 3 == 2) {
                    result.push(this.kudos[i]);
                }
            }
        }

        return result;
    }

    @computedFrom('kudos')
    get kudos3Column(): KudoViewModel[] {

        let result: KudoViewModel[] = [];
        if (this.kudos) {
            for (let i = 0; i < this.kudos.length; i++) {
                if (i % 3 == 0) {
                    result.push(this.kudos[i]);
                }
            }
        }

        return result;
    }
}