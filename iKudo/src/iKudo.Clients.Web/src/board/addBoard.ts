import { inject } from 'aurelia-framework';
import { InputsHelper } from '../inputsHelper';
import { Notifier } from '../helpers/Notifier';
import { BoardService } from '../services/boardService';

@inject(InputsHelper, Notifier, BoardService)
export class AddBoard {

    public name: string;
    public description: string;

    private notifier: Notifier;
    private inputsHelper;
    private boardService: BoardService;

    constructor(InputsHelper, notifier: Notifier, boardService: BoardService) {
        
        this.notifier = notifier;
        this.inputsHelper = InputsHelper;
        this.boardService = boardService;
    }

    submit() {
        let addCompanyUrl = 'api/board';

        let board = {
            Name: this.name,
            Description: this.description
        };

        this.boardService.add(board)
            .then(data => this.notifier.info('Dodano tablicę ' + board.Name))
            .catch(error => this.notifier.error(error));
    }

    attached() {
        this.inputsHelper.Init();
    }
}