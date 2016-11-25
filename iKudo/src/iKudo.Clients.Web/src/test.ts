import { HttpClient } from 'aurelia-fetch-client';
import { inject } from 'aurelia-framework';

@inject(HttpClient)
export class Test {

    private http: HttpClient;
    testObj: any;

    constructor(http: HttpClient) {
        this.http = http;

        this.http.configure(config => {
            config.withBaseUrl('http://localhost:49862/');
            config.withDefaults({ mode: 'cors' });
        })
    }

    getTestData() {
        let url = 'api/Test';

        this.http.fetch(url, {}).then(result => result.json()).then((result: any) => {
            console.log(result);
            this.testObj = result;
        });
    }

    public activate() {
        
        return this.getTestData();
    }
}