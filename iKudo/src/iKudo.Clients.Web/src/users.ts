import {autoinject} from 'aurelia-framework';
import {HttpClient} from 'aurelia-fetch-client';
import 'fetch';

@autoinject
export class Users {
  public heading = 'Github Users';
  public users = [];

  constructor(private http: HttpClient) {
    http.configure(config => {
      config
        .useStandardConfiguration()
          .withBaseUrl('http://localhost:49862/api');
    });
  }

  public activate() {
      return this.http.fetch('test')
          .then(response => response.json())
          //.then(users => this.users = users);
          .then(users => console.log(users));
  }
}
