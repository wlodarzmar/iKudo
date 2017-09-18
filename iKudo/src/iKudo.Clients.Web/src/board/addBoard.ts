import { inject } from 'aurelia-framework';
import { InputsHelper } from '../inputsHelper';
import { Notifier } from '../helpers/Notifier';
import { BoardService } from '../services/boardService';
import { Router } from 'aurelia-router';

@inject(InputsHelper, Notifier, BoardService, Router)
export class AddBoard {

    public name: string;
    public description: string;

    private notifier: Notifier;
    private inputsHelper;
    private boardService: BoardService;
    private router: Router;

    constructor(InputsHelper, notifier: Notifier, boardService: BoardService, router: Router) {

        this.notifier = notifier;
        this.inputsHelper = InputsHelper;
        this.boardService = boardService;
        this.router = router;
    }

    canActivate() {
        return localStorage.getItem('profile') != null;
    }

    submit() {
        let addCompanyUrl = 'api/board';

        let board = {
            Name: this.name,
            Description: this.description
        };

        this.boardService.add(board)
            .then((data : any) => {
                this.notifier.info('Dodano tablicę ' + board.Name);
                this.router.navigateToRoute("boardPreview", { id: data.id });
            })
            .catch(error => { this.notifier.error(error); });
    }

    attached() {
        this.inputsHelper.Init();
    }
}