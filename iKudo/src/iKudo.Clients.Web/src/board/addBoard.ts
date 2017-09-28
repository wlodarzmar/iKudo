import { inject, NewInstance } from 'aurelia-framework';
import { InputsHelper } from '../inputsHelper';
import { Notifier } from '../helpers/Notifier';
import { BoardService } from '../services/boardService';
import { Router } from 'aurelia-router';
import { ValidationController, ValidationRules } from 'aurelia-validation';

@inject(InputsHelper, Notifier, BoardService, Router, NewInstance.of(ValidationController))
export class AddBoard {

    public name: string;
    public description: string;

    public validation: ValidationController;
    private notifier: Notifier;
    private inputsHelper;
    private boardService: BoardService;
    private router: Router;

    constructor(InputsHelper, notifier: Notifier, boardService: BoardService, router: Router, validation: ValidationController) {

        this.notifier = notifier;
        this.inputsHelper = InputsHelper;
        this.boardService = boardService;
        this.router = router;
        this.validation = validation;

        ValidationRules.ensure('name')
            .required().withMessage('Nazwa tablicy jest obowiązkowa')
            .minLength(3).withMessage('Nazwa tablicy powinna mieć minimum 3 znaki')
            .on(this);
    }

    canActivate() {
        return localStorage.getItem('profile') != null;
    }

    attached() {
        this.inputsHelper.Init();
    }

    submit() {
        this.validation.validate()
            .then((result: any) => {
                if (result.valid) {
                    this.addBoard();
                }
            })
    }

    private addBoard() {

        let addCompanyUrl = 'api/board';

        let board = {
            Name: this.name,
            Description: this.description
        };

        this.boardService.add(board)
            .then((data: any) => {
                this.notifier.info('Dodano tablicę ' + board.Name);
                this.router.navigateToRoute("boardPreview", { id: data.id });
            })
            .catch(error => { this.notifier.error(error); });
    }
}

