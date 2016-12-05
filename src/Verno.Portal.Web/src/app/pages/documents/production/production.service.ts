import { Injectable } from '@angular/core';

import { ProductionDto } from './production.model';
import { AbpHttp } from 'app/theme/services';

@Injectable()
export class ShopService {

  private apiUrl = abp.appPath + 'api/services/app/shop/';  // URL to web api

  constructor(private http: AbpHttp) { }

  getList(shopNum: number): Promise<abp.List<ProductionDto>> {
    let url = this.apiUrl + shopNum + "/" + "production-calculator";
    return this.http.get(url)
      .toPromise();
  }
}