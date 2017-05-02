import { Injectable } from '@angular/core';
import { ResponseContentType, URLSearchParams, Headers } from '@angular/http';

import { Proekt, Filial } from './info.model';
import { AbpHttp } from '../index';

@Injectable()
export class InfoService {

  private headers = new Headers({ 'Content-Type': 'application/json' });
  private apiUrl = abp.appPath + 'api/services/info/';  // URL to web api

  constructor(private http: AbpHttp) { }

  //GET: proekts
  getProekts(): Promise<abp.List<Proekt>> {
    let url = this.apiUrl +"proekts/";
    return this.http.get(url)
      .toPromise();
  }

  //GET: filials
  getFilials(): Promise<abp.List<Filial>> {
    let url = this.apiUrl +"filials/";
    return this.http.get(url)
      .toPromise();
  }
}