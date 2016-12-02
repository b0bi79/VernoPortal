import { Injectable } from '@angular/core';

import { ProductionDto } from './production.model';
import { AbpHttp } from 'app/theme/services';

@Injectable()
export class ProductionService {

  private apiUrl = abp.appPath + 'api/services/app/shop/';  // URL to web api

  constructor(private http: AbpHttp) { }

  getList(): Promise<abp.List<ProductionDto>> {
    let url = this.apiUrl +"production-calculator";
    return this.http.get(url)
      .toPromise();
  }
}