import { HttpClient, json } from 'aurelia-fetch-client';
import { inject } from 'aurelia-framework';

@inject(HttpClient)
export class Groups {

    public groups: any;
    private http: HttpClient;

    constructor(http: HttpClient) {

        http.configure(config => {
            config.useStandardConfiguration();
            config.withBaseUrl('http://localhost:49862/');
            config.withDefaults(
                {
                    headers: {
                        'Authorization': 'Bearer ' + localStorage.getItem('id_token')
                    }
                });
        });

        this.http = http;

        http.fetch('api/group', {})
            .then(response => response.json())
            .then(data => { this.groups = data; console.log(data, 'groups'); });
    }

    delete(id: number) {
        let body = {
            method: 'DELETE',
        };

        this.http.fetch('api/group/' + id, body)
            .then(data => { console.log(data); this.removeGroup(id); alert('Usunięto grupe'); })
            .catch(error => { console.log(error); return error.json().then(e => alert(e.error)); });
    }

    private removeGroup(id: number) {

        for (let group of this.groups) {
            if (group.id == id) {
                let idx = this.groups.indexOf(group);
                if (idx != -1) {
                    this.groups.splice(idx, 1);
                }
                break;
            }
        }
    }

    edit(id: number) {
        alert(id);
    }
}