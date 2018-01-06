﻿import { inject, NewInstance, observable } from 'aurelia-framework';
import { InputsHelper } from '../inputsHelper';
import { KudoService } from '../services/kudoService';
import { User } from '../viewmodels/user';
import { KudoType } from '../viewmodels/kudoType';
import { Notifier } from '../helpers/Notifier';
import { Kudo } from '../viewmodels/kudo';
import { Router } from 'aurelia-router';
import { ValidationController, ValidationRules } from 'aurelia-validation';
import { I18N } from 'aurelia-i18n';

@inject(InputsHelper, KudoService, Notifier, Router, NewInstance.of(ValidationController), I18N)
export class AddKudo {

    public selectedType: KudoType;
    public selectedReceiver: User;
    public isAnonymous: boolean = false;
    public description: string;
    public browseButtonLabel: string;

    public types: KudoType[] = [];
    public receivers: User[] = [];
    @observable
    public selectedFiles: File[];
    private boardId: number;
    private extensions = ".jpg,.jpeg,.png,.gif";
    
    constructor(
        private readonly inputHelper: InputsHelper,
        private readonly kudoService: KudoService,
        private readonly notifier: Notifier,
        private readonly router: Router,
        private readonly validation: ValidationController,
        private readonly i18n: I18N) {

        this.inputHelper = inputHelper;
        this.kudoService = kudoService;
        this.notifier = notifier;
        this.router = router;
        this.validation = validation;
        this.i18n = i18n;

        this.initValidation();

        this.browseButtonLabel = i18n.tr('btn.select_file');
    }

    canActivate(params: any) {

        return this.kudoService.getReceivers(params.id, [])
            .then((receivers: User[]) => {
                let userId = JSON.parse(localStorage.getItem('profile')).user_id;
                let currentUserIdx: number = receivers.map(x => x.id).indexOf(userId);
                let can: boolean = currentUserIdx != -1;
                if (can) {
                    this.receivers = receivers.filter(x => x.id != userId);
                }

                return can;
            })
            .catch(() => {
                this.notifier.error(this.i18n.tr('users.fetch_error'));
                return false;
            });
    }

    activate(params: any) {

        this.kudoService.getKudoTypes()
            .then((types: KudoType[]) => this.types = types)
            .catch(() => this.notifier.error(this.i18n.tr('common.fetch_data_error')));

        this.boardId = params.id;
    }

    attached() {
        this.inputHelper.Init();
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

    get selectedFileName(): string {
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

        let userId = JSON.parse(localStorage.getItem('profile')).user_id;
        let kudo = new Kudo(
            this.boardId,
            this.selectedType,
            this.selectedReceiver && this.selectedReceiver.id || null,
            userId,
            this.description);

        kudo.isAnonymous = this.isAnonymous;
        kudo.image = await this.readSelectedFile();

        this.kudoService.add(kudo)
            .then(() => {
                this.notifier.info(this.i18n.tr('kudo.added'));
                this.router.navigateToRoute("boardPreview", { id: this.boardId });
            })
            .catch(error => this.notifier.error(error));
    }

    private async readSelectedFile() {

        return new Promise((resolve, reject) => {

            if (!this.selectedFiles) {
                resolve(null);
            }

            let reader = new FileReader();
            let file = this.selectedFiles[0];
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

    private validateFileName(name: string, obj: AddKudo): boolean {
        if (!name) {
            return true;
        }

        let isValid = false;
        obj.extensions.split(',').forEach((extension, i) => {
            if (name.endsWith(extension)) {
                isValid = true;
            }
        });

        return isValid;
    }
}