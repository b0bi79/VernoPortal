import { Injectable } from '@angular/core';
import { ResponseContentType } from '@angular/http';

import { ProductionDto } from './production.model';
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
  stickerFile(shopNum: number, vidTovara: number): Promise<Blob> {
    let url = this.apiUrl + shopNum + "/nomenklatura/" + vidTovara +"/sticker";
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