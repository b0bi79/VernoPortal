import { Injectable } from '@angular/core';
import { Http, Headers, RequestOptions, Response } from '@angular/http';

import 'rxjs/add/operator/toPromise';

import * as models from './identity.model';

function toPromise<T>($promise: abp.IGenericPromise<T>): Promise<T> {
  return new Promise<T>((resolve, reject) => {
    $promise.done(resolve).fail(reject);
  });
}

@Injectable()
export class IdentityService {

  private headers = new Headers({ 'Content-Type': 'application/json' });
  private apiUrl = abp.appPath + 'api/services/identity/account';  // URL to web api

  constructor(private http: Http) { }

  changePassword(model: models.ChangePasswordInput): Promise<void> {
    return toPromise(abp.services.identity.account.changePassword(model));
  }

  setPassword(model: models.SetPasswordInput): Promise<void> {
    return toPromise(abp.services.identity.account.setPassword(model));
  }

  hasPassword(): Promise<boolean> {
    return toPromise(abp.services.identity.account.hasPassword());
  }

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