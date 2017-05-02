import { Injectable } from '@angular/core';
import { ResponseContentType, URLSearchParams, Headers } from '@angular/http';

import { ProizvMagSpecDto, ProizvMagSpecItemDto } from './production-spec.model';
import { Nomenklatura } from 'app/theme/services/shInfoSql'
import { AbpHttp } from 'app/theme/services';

@Injectable()
export class ProductionSpecService {

  private headers = new Headers({ 'Content-Type': 'application/json' });
  private apiUrl = abp.appPath + 'api/services/app/shop/production-spec/';  // URL to web api

  constructor(private http: AbpHttp) { }

  //GET
  getList(): Promise<abp.List<ProizvMagSpecDto>> {
    let url = this.apiUrl;
    return this.http.get(url)
      .toPromise();
  }

  //GET: {specId}
  get(specId: number): Promise<ProizvMagSpecDto> {
    let url = this.apiUrl + specId;
    return this.http.get(url)
      .toPromise();
  }

  //GET: {specId}/items
  getItems(specId: number): Promise<abp.List<ProizvMagSpecItemDto>> {
    let url = this.apiUrl + specId + '/items';
    return this.http.get(url)
      .toPromise();
  }

  //GET: production-spec/names
  getNames(proekt: number): Promise<abp.List<Nomenklatura>> {
    let url = this.apiUrl + 'names/' + abp.utils.buildQueryString([{ name: "proekt", value: proekt }]);
    return this.http.get(url)
      .toPromise();
  }

  //PUT
  save(spec: ProizvMagSpecDto): Promise<ProizvMagSpecDto> {
    let url = this.apiUrl;
    return this.http.put(url, JSON.stringify(spec), { headers: this.headers })
      .toPromise();
  }

  //DELETE: {specId}/{specItemId}
  deleteItem(specId, specItemId: number): void {
    this.http.delete(this.apiUrl + specId + '/' + specItemId);
  }
}