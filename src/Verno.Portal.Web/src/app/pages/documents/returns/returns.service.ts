import { Injectable } from '@angular/core';
import { ResponseContentType, URLSearchParams } from '@angular/http';
import { Observable } from 'rxjs/Observable';

import 'rxjs/add/operator/toPromise';
import 'rxjs/add/operator/map'
import 'rxjs/add/operator/catch'
import 'rxjs/add/observable/throw';

import { ReturnDto, ReturnFileDto, RasxodLink } from './returns.model';
import { AbpHttp } from 'app/theme/services';

@Injectable()
export class ReturnsService {

  private apiUrl = abp.appPath + 'api/services/app/returns/';  // URL to web api

  constructor(private http: AbpHttp) { }

  getList(dfrom: string, dto: string, filter: string, unreclaimedOnly: boolean, shopNum?: number, filial?: number): Promise<abp.List<ReturnDto>> {
    let url = this.apiUrl + dfrom + '!' + dto +
      abp.utils.buildQueryString([
        { name: 'filter', value: filter },
        { name: 'unreclaimedOnly', value: unreclaimedOnly },
        { name: 'shopNum', value: shopNum },
        { name: 'filial', value: filial }
      ]);
    return this.http.get(url)
      .toPromise();
  }
  getFilesList(rasxod: RasxodLink): Promise<abp.List<ReturnFileDto>> {
    let params: URLSearchParams = new URLSearchParams();
    if (rasxod.shopNum) params.set('shopNum', rasxod.shopNum.toString());
    if (rasxod.docDate) params.set('docDate', rasxod.docDate.toString());
    if (rasxod.docNum) params.set('docNum', rasxod.docNum);
    if (rasxod.supplierId) params.set('supplierId', rasxod.supplierId.toString());
    if (rasxod.returnId) params.set('returnId', rasxod.returnId.toString());

    return this.http.get(this.apiUrl + 'files', { search: params })
      .toPromise();
    //return toPromise(abp.services.app.returns.getFilesList(rasxod));
  }

  deleteFile(fileId: number): Promise<ReturnFileDto> {
    return this.http.delete(this.apiUrl + 'files/' + fileId)
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