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
                        'Authorization': 'Bearer ' + localStorage.getItem('accessToken'),
                        'Cache-Control': 'no-cache',
                        'Pragma': 'no-cache',
                        'Expires': 'Sat, 01 Jan 2000 00:00:00 GMT'
                    }
                });
            config.withInterceptor({ //TODO: extract interceptors to separate file
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
                    return response;
                },
                responseError(error, response) {
                    self.requestCounter--;
                    return Promise.reject(error);
                }
            });
        });
    }

    protected async get(url: string) {

        try {
            let response = await this.http.fetch(url, {});
            let data = await response.json();
            return data;
        } catch (e) {
            return Promise.reject(await e.json());
        }
    }

    protected async post(url: string, body: any) {

        try {
            return await this.sendRequest(url, 'POST', body);
        } catch (e) {
            return Promise.reject(await e.json());
        }
    }

    protected async put(url: string, body: any) {

        try {
            return await this.sendRequest(url, 'PUT', body);
        } catch (e) {
            return Promise.reject(await e.json());
        }
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

    protected async deleteCall(url: string) { //TODO: change name
        let request = {
            method: 'DELETE'
        };

        try {
            let response = await this.http.fetch(url, request);

            let data = await this.tryParseJson(response);
            if (data) {
                return data;
            }

            return response;
        } catch (e) {
            return Promise.reject(await e.json());
        }
    }

    private async tryParseJson(obj: any) {

        try {
            let json = await obj.json();
            return json;
        } catch (e) {
            return false;
        }
    }
}