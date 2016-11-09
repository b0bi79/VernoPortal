import { Injectable } from '@angular/core';
import { Headers, Http, ResponseContentType, Response } from '@angular/http';
import { Observable } from 'rxjs/Observable';

import 'rxjs/add/operator/toPromise';
import 'rxjs/add/operator/map'
import 'rxjs/add/operator/catch'
import 'rxjs/add/observable/throw';

import { ReturnDto, ReturnFileDto } from './returns.model';
//import { AbpHttp } from 'app/theme/services';

function toPromise<T>($promise: abp.IGenericPromise<T>): Promise<T> {
  return new Promise<T>((resolve, reject) => {
    $promise.done(resolve).fail(reject);
  });
}

@Injectable()
export class ReturnsService {

  private headers = new Headers({ 'Content-Type': 'application/json' });
  private apiUrl = abp.appPath + 'api/services/app/returns/';  // URL to web api

  constructor(private http: Http) { }

  getList(dfrom: string, dto: string, filter: string, unreclaimedOnly: boolean): Promise<abp.List<ReturnDto>> {
    //return this.http.get(this.apiUrl + dfrom + '!' + dto + '?filter=' + (filter || '') + '&unreclaimedOnly=' + unreclaimedOnly)
    //  .toPromise();
    //.then(response => response.json().data as User[])
    //.catch(this.handleError);
    return toPromise(abp.services.app.returns.getList(dfrom, dto, filter, unreclaimedOnly));
  }
  getFilesList(rasxod: number): Promise<abp.List<ReturnFileDto>> {
    return toPromise(abp.services.app.returns.getFilesList(rasxod));
  }

  uploadFile(rasxod: number, file: any): Promise<ReturnFileDto> {
    return toPromise(abp.services.app.returns.uploadFile(rasxod, file));
  }

  deleteFile(fileId: number): Promise<ReturnFileDto> {
    return toPromise(abp.services.app.returns.deleteFile(fileId));
  }

  downloadPack(returnIds: number[]): Promise<Blob> {
    var self = this;
    let url = this.apiUrl + 'files/pack' + abp.utils.buildQueryString([{ name: "returnIds", value: returnIds }]);
    return this.http.get(url, { responseType: ResponseContentType.Blob })
      .map(res => res.blob())
      .catch(err => {
        self.raiseErrorResponse(err);
        return Observable.throw(err.message || err);
      })
      .toPromise();
  }

  raiseErrorResponse(response: Response): void {
    switch (response.status) {
      case 401:
        this.handleUnAuthorizedRequest(this.showError(this.defaultError401), abp.appPath);
        break;
      case 403:
        this.showError(this.defaultError403);
        break;
      case 404:
        this.showError(this.defaultError404);
        break;
      default:
        this.showError(this.defaultError);
        break;
    }
  }

  handleTargetUrl (targetUrl: string):void {
    if (!targetUrl) {
      location.href = abp.appPath;
    } else {
      location.href = targetUrl;
    }
  }

  handleUnAuthorizedRequest(messagePromise: Promise<any>, targetUrl: string) {
    if (messagePromise) {
      messagePromise.then(() => {
        this.handleTargetUrl(targetUrl);
      });
    } else {
      this.handleTargetUrl(targetUrl);
    }
  }

  defaultError = {
    message: 'Произошла ошибка!',
    details: 'Информация об ошибке сервером не была предоставлена.',
  }

  defaultError401 = {
    message: 'Вы не авторизованы!',
    details: 'Вы должны пройти проверку подлинности (войти) для того, чтобы выполнить эту операцию.'
  }

  defaultError403 = {
    message: 'Вы не авторизованы!',
    details: 'Вы не можете выполнить эту операцию.'
  }

  defaultError404 = {
    message: 'Ресурсы не найдены!',
    details: 'Запрошенный ресурс не найден на сервере.'
  }

  logError(error) {
    abp.log.error(error);
  }

  showError (error): Promise<any> {
    if (error.details) {
      return toPromise(abp.message.error(error.details, error.message));
    } else {
      return toPromise(abp.message.error("", error.message || this.defaultError.message));
    }
  }
}