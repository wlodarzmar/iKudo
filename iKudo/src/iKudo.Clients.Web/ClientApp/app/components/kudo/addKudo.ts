import { inject, NewInstance, observable } from 'aurelia-framework';
import { InputsHelper } from '../../helpers/inputsHelper';
import { KudoService } from '../../services/kudoService';
import { User } from '../../viewmodels/user';
import { KudoType } from '../../viewmodels/kudoType';
import { Notifier } from '../helpers/Notifier';
import { Kudo } from '../../viewmodels/kudo';
import { Router } from 'aurelia-router';
import { ValidationController, ValidationRules } from 'aurelia-validation';
import { I18N } from 'aurelia-i18n';
import { BoardService } from '../../services/boardService';
import { ViewModelBase } from '../../viewmodels/viewModelBase';

@inject(InputsHelper, KudoService, Notifier, Router, NewInstance.of(ValidationController), I18N, BoardService)
export class AddKudo extends ViewModelBase {

    public selectedType: KudoType;
    public selectedReceiver: User;
    public isAnonymous: boolean = false;
    public description: string;
    public browseButtonLabel: string;
    public isEnabled = true;

    public types: KudoType[] = [];
    public receivers: User[] = [];
    @observable
    public selectedFiles: File[] | null;
    private boardId: number;
    private extensions = ".jpg,.jpeg,.png,.gif";

    constructor(
        private readonly inputHelper: InputsHelper,
        private readonly kudoService: KudoService,
        private readonly notifier: Notifier,
        private readonly router: Router,
        private readonly validation: ValidationController,
        private readonly i18n: I18N,
        private readonly boardService: BoardService) {

        super();
        this.inputHelper = inputHelper;
        this.kudoService = kudoService;
        this.notifier = notifier;
        this.router = router;
        this.validation = validation;
        this.i18n = i18n;

        this.initValidation();

        this.browseButtonLabel = i18n.tr('btn.select_file');
    }

    async canActivate(params: any) {

        try {
            let board: any = await this.boardService.get(params.id);
            let receivers: User[] = await this.kudoService.getReceivers(params.id, []);
            let can: boolean = !board.isPrivate;
            if (board.isPrivate) {
                let currentUserIdx: number = receivers.map(x => x.id).indexOf(this.currentUserId || '');
                can = currentUserIdx != -1;
            }
            if (can) {
                this.receivers = receivers.filter(x => x.id != this.currentUserId);
            }

            console.log(can, 'can?');

            return can;

        } catch (e) {
            this.notifier.error(this.i18n.tr('users.fetch_error'));
        }
    }

    activate(params: any) {

        this.kudoService.getKudoTypes()
            .then((types: KudoType[]) => this.types = types)
            .catch(() => this.notifier.error(this.i18n.tr('common.fetch_data_error')));

        this.boardId = params.id;
        console.log('actviate');
    }

    attached() {
        console.log('att');
        this.inputHelper.Init();
        console.log('att2');
    }

    submit() {

        this.validation.validate()
            .then((result: any) => {
                if (result.valid) {
                    this.addKudo();
                }
            })
    }

    clearPreview() {
        this.selectedFiles = null;
    }

    get selectedFileName(): string | null {
        if (this.selectedFiles != null && this.selectedFiles[0] != null) {
            return this.selectedFiles[0].name;
        }
        else {
            return null;
        }
    }

    private async selectedFilesChanged(newValue: File, oldValue: File) {

        let result = {};
        if (this.selectedFiles && this.selectedFiles[0]) {
            this.browseButtonLabel = this.selectedFiles[0].name;
            result = await this.readSelectedFile();
            $('#file_preview').attr('src', result as string);
        }
        else {
            this.browseButtonLabel = this.i18n.tr('btn.select_file');
            $('#file_preview').removeAttr('src');
        }
    }

    private async addKudo() {

        this.isEnabled = false;
        let kudo = await this.getKudoFromViewModel();

        try {
            await this.kudoService.add(kudo);
            this.notifier.info(this.i18n.tr('kudo.added'));
            this.router.navigateToRoute("boardPreview", { id: this.boardId });
        } catch (e) {
            this.notifier.error(e);
            this.isEnabled = true;
        }
    }

    private async getKudoFromViewModel() {
        let kudo = new Kudo(
            this.boardId,
            this.selectedType,
            this.selectedReceiver && this.selectedReceiver.id || "",
            this.currentUserId || '',
            this.description);

        kudo.isAnonymous = this.isAnonymous;
        kudo.image = await this.readSelectedFile();

        return kudo;
    }

    private async readSelectedFile() {

        return new Promise((resolve, reject) => {

            if (!this.selectedFiles) {
                resolve(undefined);
            }

            let reader = new FileReader();
            let file = this.selectedFiles![0];
            reader.readAsDataURL(file);
            reader.onload = () => {
                resolve(reader.result);
            };
            reader.onerror = () => {
                reject(reader.error);
            }
        });
    }

    private initValidation() {
        ValidationRules
            .ensure('selectedReceiver').required().withMessage(this.i18n.tr('kudo.receiver_is_required'))
            .ensure('selectedType').required().withMessage(this.i18n.tr('kudo.type_is_required'))
            .ensure('selectedFileName').satisfies(this.validateFileName).withMessage(this.i18n.tr('errors.wrong_extension_error', { extensions: this.extensions }))
            .on(this);
    }

    private validateFileName(name: string, obj: any): boolean | Promise<boolean> {
        if (!name) {
            return true;
        }

        let isValid = false;
        obj.extensions.split(',').forEach((extension: string, i: number) => {
            if (name.toLowerCase().endsWith(extension.toLowerCase())) {
                isValid = true;
            }
        });

        return isValid;
    }
}