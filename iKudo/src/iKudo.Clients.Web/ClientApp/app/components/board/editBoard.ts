import { inject } from 'aurelia-framework';
import { InputsHelper } from '../../helpers/inputsHelper';
import { Notifier } from '../helpers/Notifier';
import { BoardService } from '../../services/boardService';
import { Router } from 'aurelia-router';
import { I18N } from 'aurelia-i18n';
import { ViewModelBase } from '../../viewmodels/viewModelBase';

@inject(InputsHelper, Notifier, BoardService, Router, I18N)
export class EditBoard extends ViewModelBase {

    public name: string;
    public description: string;
    public id: number;
    public creatorId: string;
    public creationDate: Date

    private inputsHelper: InputsHelper;
    private notifier: Notifier;
    private boardService: BoardService;
    private router: Router;
    private i18n: I18N;

    constructor(inputsHelper: InputsHelper, notifier: Notifier, boardService: BoardService, router: Router, i18n: I18N) {

        super();
        this.inputsHelper = inputsHelper;
        this.notifier = notifier;
        this.boardService = boardService;
        this.router = router;
        this.i18n = i18n;
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
    }

    submit() {

        // TODO: dodać taki model
        let board = {
            Id: this.id,
            CreatorId: this.creatorId,
            Name: this.name,
            Description: this.description,
            CreationDate: this.creationDate
        };

        this.boardService.edit(board)
            .then(() => {
                this.notifier.info(this.i18n.tr('boards.changes_saved', {name: board.Name}));
                this.router.navigateToRoute("boardPreview", { id: board.Id });
            })
            .catch(error => this.notifier.error(error));
    }
}