import { Api } from './api';
import { json } from 'aurelia-fetch-client';
import { inject } from 'aurelia-framework';

//@inject(Api)
export class BoardService extends Api {

    //private api: Api;

    //constructor(api: Api) {

    //    this.api = api;
    //}

    public edit(board: any) {
        
        let requestBody = {
            method: 'PUT',
            body: json(board)
        };
        
        return this.http.fetch('api/board', requestBody);
    }
}