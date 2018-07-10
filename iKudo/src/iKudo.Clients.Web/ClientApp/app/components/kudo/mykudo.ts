import { inject } from 'aurelia-framework';
import { KudoService } from '../../services/kudoService';
import { Notifier } from '../../helpers/Notifier';
import { Kudo } from '../../viewmodels/kudo';
import { KudoViewModel } from '../../viewmodels/kudoViewModel';
import { ViewModelBase } from '../../viewmodels/viewModelBase';
import { I18N } from 'aurelia-i18n';
import { AuthService } from "../../services/authService";
import { User } from "../../services/models/user";

@inject(KudoService, Notifier, I18N, AuthService)
export class MyKudo extends ViewModelBase {

    constructor(
        private readonly kudoService: KudoService,
        private readonly notifier: Notifier,
        private readonly i18n: I18N,
        private readonly authService: AuthService) {

        super();
    }

    public sent: boolean = true;
    public received: boolean = true;
    public kudos: KudoViewModel[] = [];

    canActivate() {
        return this.currentUserId != undefined;
    }

    activate() {

        return this.findKudosByCriteria();
    }

    public submit() {

        this.findKudosByCriteria();
    }

    private async findKudosByCriteria() {

        if (!this.currentUserId) {
            return;
        }

        try {
            let kudos = await this.kudoService.getKudos(null, this.currentUserId, this.sent, this.received);
            this.kudos = kudos.map(x => {
                let user = this.authService.getUser() || new User();
                return KudoViewModel.convert(x, user.name);
            });
        } catch (e) {
            this.notifier.error(this.i18n.tr('kudo.fetch_error'));
        }
    }

    public refreshSearch() {
        this.sent = true;
        this.received = true;
    }
}