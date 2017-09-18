import { inject } from 'aurelia-framework';
import { HttpClient, json } from 'aurelia-fetch-client';

@inject(HttpClient)
export class Api {

    protected http: HttpClient;
    private requestCounter: number = 0;
    
    constructor(http: HttpClient) {

        let self = this;

        http.configure(config => {
            config.useStandardConfiguration();
            config.withBaseUrl('http://localhost:49862/');
            config.withDefaults(
                {
                    mode: 'cors',
                    headers: {
                        'Authorization': 'Bearer ' + localStorage.getItem('id_token')
                    }
                });
            config.withInterceptor({
                request(request) {

                    self.requestCounter++;
                    console.log(`Requesting ${request.method} ${request.url}`);

                    if (request.headers.has('Authorization')) {
                        request.headers.delete('Authorization');
                    }
                    request.headers.append('Authorization', 'Bearer ' + localStorage.getItem('id_token'));

                    return request;
                },
                response(response) {

                    self.requestCounter--;
                    console.log(`Received ${response.status} ${response.url}`);
                    return response;
                },
                responseError(error, response) {

                    self.requestCounter--;
                    console.log(error, 'Received error');
                    return Promise.reject(error);
                }
            });
        });

        this.http = http;
    }

    public get isRequesting(): boolean {
        return this.requestCounter > 0;
    }
}