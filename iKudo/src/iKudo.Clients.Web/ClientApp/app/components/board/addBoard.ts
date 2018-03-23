import { inject, NewInstance } from 'aurelia-framework';
import { InputsHelper } from '../../helpers/inputsHelper';
import { Notifier } from '../helpers/Notifier';
import { BoardService } from '../../services/boardService';
import { Router } from 'aurelia-router';
import { ValidationController, ValidationRules } from 'aurelia-validation';
import { I18N } from 'aurelia-i18n';

@inject(InputsHelper, Notifier, BoardService, Router, NewInstance.of(ValidationController), I18N)
export class AddBoard {

    public name: string;
    public description: string;

    public validation: ValidationController;
    private notifier: Notifier;
    private inputsHelper: any;
    private boardService: BoardService;
    private router: Router;
    private i18n: I18N;

    constructor(inputsHelper: InputsHelper, notifier: Notifier, boardService: BoardService, router: Router, validation: ValidationController, i18n: I18N) {

        this.notifier = notifier;
        this.inputsHelper = inputsHelper;
        this.boardService = boardService;
        this.router = router;
        this.validation = validation;
        this.i18n = i18n;

        ValidationRules.ensure('name')
            .required().withMessage(i18n.tr('boards.name_is_required'))
            .minLength(3).withMessage(i18n.tr('boards.name_min_length', { min: 3 }))
            .on(this);
    }

    canActivate() {
        console.log('can or not?');
        return localStorage.getItem('profile') != null;
    }

    attached() {
        console.log('att');
        this.inputsHelper.Init();
        console.log('att2');
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
                this.notifier.info(this.i18n.tr('boards.added_info', { name: board.Name }));
                this.router.navigateToRoute("boardPreview", { id: data.id });
            })
            .catch(error => { this.notifier.error(error); });
    }
}

