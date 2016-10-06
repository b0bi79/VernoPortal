﻿import { Injectable } from '@angular/core';
import { Http, Headers } from '@angular/http';

import 'rxjs/add/operator/toPromise';

import { OrgUnit } from './users.model';

function toPromise<T>($promise: abp.IGenericPromise<T>): Promise<T> {
  return new Promise<T>((resolve, reject) => {
    $promise.done(resolve).fail(reject);
  });
}

@Injectable()
export class OrgUnitService {

  private headers = new Headers({ 'Content-Type': 'application/json' });
  private apiUrl = abp.appPath + 'api/services/identity/orgUnit';  // URL to web api

  constructor(private http: Http) { }

  getAll(): Promise<abp.List<OrgUnit>> {
    return toPromise(abp.services.identity.orgUnit.getAll());
    /*return this.http.get(this.apiUrl)
        .toPromise()
        .then(response => response.json().data as User[])
        .catch(this.handleError);*/
  }

  private handleError(error: any): Promise<any> {
    console.log('Произошла ошибка', error);
    return Promise.reject(error.message || error);
  }
}