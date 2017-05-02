import { Injectable } from '@angular/core';
import { ResponseContentType, URLSearchParams } from '@angular/http';

import { ProductionDto, IdQty } from './production.model';
import { AbpHttp } from 'app/theme/services';

@Injectable()
export class ShopService {

  private apiUrl = abp.appPath + 'api/services/app/shop/';  // URL to web api

  constructor(private http: AbpHttp) { }

  //{shopNum}/production-calculator
  getList(shopNum: number): Promise<abp.List<ProductionDto>> {
    let url = this.apiUrl + shopNum + "/production-calculator";
    return this.http.get(url)
      .toPromise();
  }

  //{shopNum}/nomenklatura/{vidTovara}/sticker
  /*stickerFile(shopNum: number, vidTovara: number): Promise<Blob> {
    let url = this.apiUrl + shopNum + "/nomenklatura/" + vidTovara +"/sticker";
    return this.http.get(url, { responseType: ResponseContentType.Blob })
      .map(res => res.blob())
      .toPromise();
  }*/

  //{shopNum}/nomenklatura/sticker
  stickerFile(shopNum: number, toPrint: IdQty[]): Promise<Blob> { //{ id: number, qty: number }[]): Promise<Blob> {
    let params = [
      { name: "ids", value: toPrint.map(x => x.id) },
      { name: "qtys", value: toPrint.map(x => x.qty) }
    ];
    let url = this.apiUrl + shopNum + "/nomenklatura/sticker" + abp.utils.buildQueryString(params);
    return this.http.get(url, { responseType: ResponseContentType.Blob })
      .map(res => res.blob())
      .toPromise();
  }

  //{shopNum}/production-calculator/excel
  getListExcel(shopNum: number): Promise<Blob> {
    let url = this.apiUrl + shopNum + "/production-calculator/excel";
    return this.http.get(url, { responseType: ResponseContentType.Blob })
      .map(res => res.blob())
      .toPromise();
  }
}