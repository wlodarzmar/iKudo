import { inject } from 'aurelia-framework';
import { HttpClient, json } from 'aurelia-fetch-client';
import * as $ from 'jquery';


@inject(HttpClient)
export class Api {

    private requestCounter: number = 0;

    constructor(
        protected readonly http: HttpClient) {

        let self = this;

        http.configure(config => {
            config.useStandardConfiguration();
            let baseUrl: string = $('#baseUrl').val() as string;
            config.withBaseUrl(baseUrl);
            config.withDefaults(
                {
                    mode: 'cors',
                    headers: {
                        'Authorization': 'Bearer ' + localStorage.getItem('accessToken')
                    }
                });
            config.withInterceptor({
                request(request) {

                    self.requestCounter++;
                    console.log(`Requesting ${request.method} ${request.url}`);

                    if (request.headers.has('Authorization')) {
                        request.headers.delete('Authorization');
                    }
                    request.headers.append('Authorization', 'Bearer ' + localStorage.getItem('accessToken'));
                    return request;
                },
                response(response) {

                    self.requestCounter--;
                    console.log(`Received ${response.status} ${response.url}`);
                    //console.log(response, 'RESPONSE');

                    return response;
                },
                responseError(error, response) {

                    self.requestCounter--;
                    //console.log(error, 'Received error');
                    return Promise.reject(error);
                }
            });
        });
    }

    protected async get(url: string) {

        let response = await this.http.fetch(url, {});
        let data = await response.json();
        return data;
    }

    protected async post(url: string, body: any) {

        return await this.sendRequest(url, 'POST', body);
    }

    protected async put(url: string, body: any) {

        return await this.sendRequest(url, 'PUT', body);
    }

    private async sendRequest(url: string, httpMethod: string, body: any) {

        let requestBody = {
            method: httpMethod,
            body: json(body)
        };

        let response = await this.http.fetch(url, requestBody);

        let data = await this.tryParseJson(response);
        if (data) {
            return data;
        }

        return response;
    }

    protected async deleteCall(url: string) {
        let request = {
            method: 'DELETE'
        };

        let response = await this.http.fetch(url, request);

        let data = await this.tryParseJson(response);
        if (data) {
            return data;
        }

        return response;
    }

    private async tryParseJson(obj: any) {

        try {
            let json = await obj.json();
            return json;
        } catch (e) {
            return false;
        }
    }

    protected get isRequesting(): boolean {
        return this.requestCounter > 0;
    }
}