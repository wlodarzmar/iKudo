import { User } from "./user";

export class AuthenticationChangedEventData {
    isAuthenticated: boolean;

    user: User;
}