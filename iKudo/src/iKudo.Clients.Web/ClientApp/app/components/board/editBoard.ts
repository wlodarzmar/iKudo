import { inject } from 'aurelia-framework';
import { InputsHelper } from '../../helpers/inputsHelper';
import { Notifier } from '../../helpers/Notifier';
import { BoardService } from '../../services/boardService';
import { Router } from 'aurelia-router';
import { I18N } from 'aurelia-i18n';
import { ViewModelBase } from '../../viewmodels/viewModelBase';
import { ValidationController, ValidationRules, validateTrigger } from 'aurelia-validation';
import { Board } from "../../services/models/board";

@inject(InputsHelper, Notifier, BoardService, Router, I18N, ValidationController)
export class EditBoard extends ViewModelBase {

    public board: Board;

    constructor(
        private readonly inputsHelper: InputsHelper,
        private readonly notifier: Notifier,
        private readonly boardService: BoardService,
        private readonly router: Router,
        private readonly i18n: I18N,
        private readonly validationController: ValidationController) {

        super();
    }

    async canActivate(params: any) {

        try {
            let board = await this.boardService.find(params.id);
            let can = this.currentUserId == board.creatorId;
            if (can) {
                this.board = board;
            }
            return can;
        } catch (e) {
            this.notifier.error(e);
            return false;
        }
    }

    attached() {

        this.inputsHelper.Init();
        this.initValidation();
    }

    private initValidation() {
        //TODO: exclude validation from here and from addBoard
        ValidationRules.ensure((board: Board) => board.name)
            .required().withMessage(this.i18n.tr('boards.name_is_required'))
            .minLength(3).withMessage(this.i18n.tr('boards.name_min_length', { min: 3 }))
            .on(this.board);
    }

    async submit() {

        try {

            let validationResult = await this.validationController.validate();
            if (validationResult.valid) {
                await this.boardService.edit(this.board);
                this.notifier.info(this.i18n.tr('boards.changes_saved', { name: this.board.name }));
                this.router.navigateToRoute("boardPreview", { id: this.board.id });
            }
        } catch (e) {
            console.log(e, 'EDIT error');
            this.notifier.error(e.message);
        }
    }
}