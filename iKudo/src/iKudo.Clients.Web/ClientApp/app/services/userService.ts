import { User } from "./models/user";
import { Api } from "./api";
import { json, HttpClient } from "aurelia-fetch-client";
import { inject } from "aurelia-framework";
import { ErrorParser } from "./errorParser";

@inject(HttpClient, ErrorParser)
export class UserService extends Api {

    constructor(
        private readonly httpClient: HttpClient,
        private readonly errorParser: ErrorParser
    ) {
        super(httpClient);
    }

    public async addOrUpdate(user: User) {

        return await this.put('api/users', user);
    }
}