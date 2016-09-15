import { Injectable } from '@angular/core';
import { Http, Headers, RequestOptions, Response } from '@angular/http';

import 'rxjs/add/operator/toPromise';


@Injectable()
export class IdentityService {
    constructor(private http: Http) { }

    logout(): Promise<any> {
        return this.http.get(abp.appPath + "Account/LogOff")
            .toPromise()
            .then(res => res.json().data)
            .catch(this.handleError);
    }

    private post(url: string, val: any = null): Promise<any> {
        var body = val != null ? JSON.stringify(val) : null;
        let headers = new Headers({ 'Content-Type': 'application/json' });
        let options = new RequestOptions({ headers });

        return this.http.post(url, body, options)
            .toPromise()
            .then(res => res.json().data)
            .catch(this.handleError);
    }

    private handleError(error: any): Promise<any> {
        console.log('Произошла ошибка', error);
        return Promise.reject(error.message || error);
    }
}