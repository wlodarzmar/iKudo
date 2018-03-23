import { inject, NewInstance } from 'aurelia-framework';
import { InputsHelper } from '../../helpers/inputsHelper';
import { Notifier } from '../helpers/Notifier';
import { BoardService } from '../../services/boardService';
import { Router } from 'aurelia-router';
import { ValidationController, ValidationRules } from 'aurelia-validation';
import { I18N } from 'aurelia-i18n';
import { ViewModelBase } from "../../viewmodels/viewModelBase";

@inject(InputsHelper, Notifier, BoardService, Router, NewInstance.of(ValidationController), I18N)
export class Add extends ViewModelBase {

    constructor(
        private readonly inputsHelper: InputsHelper,
        private readonly notifier: Notifier,
        private readonly boardService: BoardService,
        private readonly router: Router,
        private readonly validation: ValidationController,
        private readonly i18n: I18N) {

        super();

        ValidationRules.ensure('name')
            .required().withMessage(i18n.tr('boards.name_is_required'))
            .minLength(3).withMessage(i18n.tr('boards.name_min_length', { min: 3 }))
            .on(this);
    }

    public name: string | undefined = undefined;
    public description: string | undefined = undefined;

    async submit() {

        let validationResult = await this.validation.validate();
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

        let boardToAdd = {
            Name: this.name,
            Description: this.description
        };

        return await this.boardService.add(boardToAdd);
    }
}