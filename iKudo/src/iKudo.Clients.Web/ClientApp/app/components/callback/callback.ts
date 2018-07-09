import { inject } from "aurelia-framework";
import { AuthService } from "../../services/authService";

@inject(AuthService)
export class Callback {

    constructor(private readonly authService: AuthService) {
        
        this.authService.handleAuthentication();
    }
}