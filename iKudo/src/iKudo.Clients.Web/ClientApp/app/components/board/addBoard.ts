import { inject, NewInstance } from 'aurelia-framework';
import { InputsHelper } from '../../helpers/inputsHelper';
import { Notifier } from '../helpers/Notifier';
import { BoardService } from '../../services/boardService';
import { Router } from 'aurelia-router';
import { I18N } from 'aurelia-i18n';
import { ViewModelBase } from '../../viewmodels/viewModelBase';
import { ValidationController, ValidationRules, validateTrigger } from 'aurelia-validation';
import { Board } from '../../services/models/board';


@inject(InputsHelper, Notifier, BoardService, Router, I18N, NewInstance.of(ValidationController))
export class AddBoard extends ViewModelBase {

    public board: Board;

    constructor(
        private readonly inputsHelper: InputsHelper,
        private readonly notifier: Notifier,
        private readonly boardService: BoardService,
        private readonly router: Router,
        private readonly i18n: I18N,
        private readonly validationController: ValidationController) {

        super();

        validationController.validateTrigger = validateTrigger.blur;
        this.board = new Board();
    }

    canActivate() {
        return this.currentUserId != undefined;
    }

    activate() {

        ValidationRules.ensure((board: Board) => board.name)
            .required().withMessage(this.i18n.tr('boards.name_is_required'))
            .minLength(3).withMessage(this.i18n.tr('boards.name_min_length', { min: 3 }))
            .on(this.board);
    }

    attached() {
        this.inputsHelper.Init();
    }

    async submit() {
        let validationResult = await this.validationController.validate();
        if (validationResult.valid) {
            try {
                let board: any = await this.addBoard();
                this.notifier.info(this.i18n.tr('boards.added_info', { name: board.name }));
                this.router.navigateToRoute("boardPreview", { id: board.id });
            } catch (e) {
                this.notifier.error(e);
            }
        }
    }

    private async addBoard() {

        return await this.boardService.add(this.board);
    }
}

