import { computedFrom, inject } from 'aurelia-framework';
import { BoardService } from '../services/boardService'
import { KudoService } from '../services/kudoService'

@inject(BoardService, KudoService)
export class Preview {
    constructor(boardService: BoardService, kudoService: KudoService) {

        this.boardService = boardService;
        this.kudoService = kudoService;

        for (let i = 0; i < 5; i++) {

            this.kudos.push(
                new KudoViewModel(1, "Good Job", "Bardzo dobra robota oby tak dalej", new Date(), "Marian Paździoch", "Ferdek Kiepski"),
                new KudoViewModel(2, "Gratulacje", "Gratuluję awansu, należało Ci się", new Date(), "Paweł Groński", "Marcin Włodarz"),
                new KudoViewModel(3, "Totally Awsome", "Świetna inicjatywa, zamierzam się przyłączyć", new Date(), "Marcin Włodarz", "Paweł Groński"),
                new KudoViewModel(4, "Gratulacje", "Gratuluję awansu, należało Ci się. Lorem ipsum tere fere kuku ryku jaja baranie hej!!!!", new Date(), "Paweł Groński", "Marcin Włodarz"),
                new KudoViewModel(4, "Gratulacje", "Gratuluję awansu, należało Ci się. Lorem ipsum tere fere kuku ryku jaja baranie hej!!!!", new Date(), "Paweł Groński", "Marcin Włodarz"),
                new KudoViewModel(5, "Totally Awsome", "Świetna inicjatywa, zamierzam się przyłączyć", new Date(), "Marcin Włodarz", "Paweł Groński"),
                new KudoViewModel(6, "Good Job", "Bardzo dobra robota oby tak dalej", new Date(), "Marian Paździoch", "Ferdek Kiepski"),
                new KudoViewModel(7, "Thank you", "Dziękuj za pomoc przy wdrożeniu do projektu", new Date(), "Marcin Włodarz", "Jan Nowak"),
            );
        }
    }

    private boardService: BoardService;
    private kudoService: KudoService;
    public name: string = "Board naaammmmeee";
    public id: number;
    public kudos: KudoViewModel[] = [];

    activate(params: any) {
        this.id = params.id;

        this.boardService.getWithUsers(params.id).then(board => console.log(board, 'BOARD'));
        //this.kudoService.getReceivers(params.id
    }

    @computedFrom('kudos')
    get kudos1Column(): KudoViewModel[] {

        let result: KudoViewModel[] = [];
        for (let i = 0; i < this.kudos.length; i++) {
            if (i % 3 == 1) {
                result.push(this.kudos[i]);
            }
        }

        return result;
    }

    @computedFrom('kudos')
    get kudos2Column(): KudoViewModel[] {

        let result: KudoViewModel[] = [];
        for (let i = 0; i < this.kudos.length; i++) {
            if (i % 3 == 2) {
                result.push(this.kudos[i]);
            }
        }

        return result;
    }

    @computedFrom('kudos')
    get kudos3Column(): KudoViewModel[] {

        let result: KudoViewModel[] = [];
        for (let i = 0; i < this.kudos.length; i++) {
            if (i % 3 == 0) {
                result.push(this.kudos[i]);
            }
        }

        return result;
    }
}

export class KudoViewModel {

    constructor(id: number, type: string, text: string, date: Date, sender: string, receiver: string) {

        this.id = id;
        this.type = type;
        this.text = text;
        this.creationDate = date;
        this.sender = sender;
        this.receiver = receiver;
    }

    public id: number;
    public type: string;
    public text: string;
    public creationDate: Date;
    public sender: string;
    public receiver: string; 
}