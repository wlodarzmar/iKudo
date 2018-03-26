import { inject } from 'aurelia-framework';
import { InputsHelper } from '../../helpers/inputsHelper';
import { Notifier } from '../helpers/Notifier';
import { BoardService } from '../../services/boardService';
import { Router } from 'aurelia-router';
import { I18N } from 'aurelia-i18n';
import { ViewModelBase } from '../../viewmodels/viewModelBase';
import { ValidationController, ValidationRules, validateTrigger } from 'aurelia-validation';

@inject(InputsHelper, Notifier, BoardService, Router, I18N, ValidationController)
export class EditBoard extends ViewModelBase {

    public name: string;
    public description: string;
    public id: number;
    public creatorId: string;
    public creationDate: Date

    constructor(
        private readonly inputsHelper: InputsHelper,
        private readonly notifier: Notifier,
        private readonly boardService: BoardService,
        private readonly router: Router,
        private readonly i18n: I18N,
        private readonly validationController: ValidationController) {

        super();
    }

    canActivate(params: any) {

        return new Promise((resolve, reject) => {

            //TODO: pobiera się cała tablica, może warto byłoby pobierać tylko creatorId?
            this.boardService.get(params.id)
                .then((board: any) => {
                    let userProfile = JSON.parse(localStorage.getItem('profile') || "");
                    let can = this.currentUserId == board.creatorId;
                    resolve(can);
                })
                .catch(error => {
                    console.log(error);
                    resolve(false);
                });
        });
    }

    activate(params: any) {

        this.boardService.get(params.id)
            .then((board: any) => {

                this.name = board.name;
                this.description = board.description;
                this.id = board.id;
                this.creatorId = board.creatorId;
                this.creationDate = board.creationDate;

                setTimeout(() => this.inputsHelper.Init(), 100);
            })
            .catch(error => this.notifier.error(error));

        this.initValidation();
    }

    private initValidation() {
        //TODO: exclude validation from here and from addBoard
        ValidationRules.ensure('name')
            .required().withMessage(this.i18n.tr('boards.name_is_required'))
            .minLength(3).withMessage(this.i18n.tr('boards.name_min_length', { min: 3 }))
            .on(this);
    }

    async submit() {

        // TODO: dodać taki model
        let board = {
            Id: this.id,
            CreatorId: this.creatorId,
            Name: this.name,
            Description: this.description,
            CreationDate: this.creationDate
        };

        let validationResult = await this.validationController.validate();
        if (validationResult.valid) {

            this.boardService.edit(board)
                .then(() => {
                    this.notifier.info(this.i18n.tr('boards.changes_saved', { name: board.Name }));
                    this.router.navigateToRoute("boardPreview", { id: board.Id });
                })
                .catch(error => this.notifier.error(error));
        }
    }
}