import { inject } from 'aurelia-framework';
import { AuthService } from '../../../services/authService';
import { AuthLoginOptions } from "../../../services/models/auth-login-options.model";
import { Router } from 'aurelia-router';

@inject(AuthService, Router)
export class LoginComponent {

    constructor(
        private readonly authService: AuthService,
        private readonly router: Router
    ) {
    }

    activate() {
        if (!this.authService.isAuthenticated) {
            this.authService.login();
        }
        else {
            this.router.navigate('dashboard');
        }
    }
}