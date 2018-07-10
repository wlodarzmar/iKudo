import { User } from "./models/user";
import { Api } from "./api";
import { json, HttpClient } from "aurelia-fetch-client";
import { inject } from "aurelia-framework";

@inject(HttpClient)
export class UserService extends Api {

    constructor(
        private readonly httpClient: HttpClient,
    ) {
        super(httpClient);
    }

    public async addOrUpdate(user: User) {

        return await this.put('api/users', user);
    }
}