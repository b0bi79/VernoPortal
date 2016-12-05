import { Injectable } from '@angular/core';
import { Headers, Http, ResponseContentType, Response } from '@angular/http';
import { Observable } from 'rxjs/Observable';

import 'rxjs/add/operator/toPromise';
import 'rxjs/add/operator/map'
import 'rxjs/add/operator/catch'
import 'rxjs/add/observable/throw';

import { ReturnDto, ReturnFileDto } from './returns.model';
import { AbpHttp } from 'app/theme/services';

function toPromise<T>($promise: abp.IGenericPromise<T>): Promise<T> {
  return new Promise<T>((resolve, reject) => {
    $promise.done(resolve).fail(reject);
  });
}

@Injectable()
export class ReturnsService {

  private apiUrl = abp.appPath + 'api/services/app/returns/';  // URL to web api

  constructor(private http: AbpHttp) { }

  getList(dfrom: string, dto: string, filter: string, unreclaimedOnly: boolean, shopNum?: number): Promise<abp.List<ReturnDto>> {
    let url = this.apiUrl + dfrom + '!' + dto +
      abp.utils.buildQueryString([
        { name: 'filter', value: filter },
        { name: 'unreclaimedOnly', value: unreclaimedOnly },
        { name: 'shopNum', value: shopNum }
      ]);
    return this.http.get(url)
      .toPromise();
//    return toPromise(abp.services.app.returns.getList(dfrom, dto, filter, unreclaimedOnly, shopNum));
  }
  getFilesList(rasxod: number): Promise<abp.List<ReturnFileDto>> {
    return this.http.get(this.apiUrl + rasxod + '/files')
      .toPromise();
    //return toPromise(abp.services.app.returns.getFilesList(rasxod));
  }

  deleteFile(fileId: number): Promise<ReturnFileDto> {
    return this.http.delete(this.apiUrl + '/files/' + fileId)
      .toPromise();
    //return toPromise(abp.services.app.returns.deleteFile(fileId));
  }

  downloadPack(returnIds: number[]): Promise<Blob> {
    let url = this.apiUrl + 'files/pack' + abp.utils.buildQueryString([{ name: "returnIds", value: returnIds }]);
    return this.http.get(url, { responseType: ResponseContentType.Blob })
      .map(res => res.blob())
      .toPromise();
  }
}