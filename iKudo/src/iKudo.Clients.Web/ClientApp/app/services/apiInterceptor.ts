export class ApiInterceptor {

    public requestCounter: number = 0;

    request(request: any) {

        this.requestCounter++;
        console.log(`Requesting ${request.method} ${request.url}`);

        if (request.headers.has('Authorization')) {
            request.headers.delete('Authorization');
        }
        request.headers.append('Authorization', 'Bearer ' + localStorage.getItem('accessToken'));

        return request;
    }

    response(response: any) {

        this.requestCounter--;
        console.log(`Received ${response.status} ${response.url}`);
        return response;
    }

    responseError(error: any, response: any) {
        this.requestCounter--;
        return Promise.reject(error);
    }
}