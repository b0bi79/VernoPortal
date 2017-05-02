import * as moment from 'moment';import { JsonProperty } from "app/utils/mapping-json";

export interface ReturnDto extends abp.services.app.IReturnDto {
}
export interface RasxodLink extends abp.services.app.IRasxodLink {
}
export interface ReturnFileDto extends abp.services.app.IReturnFileDto {
}
export class Return implements ReturnDto, RasxodLink {
  returnId?: number = undefined;
  shopNum: number = undefined;
  docDate: Date = undefined;
  docNum: string = undefined;
  supplierId: number = undefined;
  supplierName: string = undefined;
  summ: number = undefined;
  liniah: number = undefined;
  liniahTip: string = undefined;

  @JsonProperty('status')
  private _status: number = undefined;
  public get status(): number {
    if (this.files == undefined)
      return this._status;
    else
      return this.files.length === 0 ? 0 : 10;
  }
  public set status(value: number) {
    this._status = value;
  }
  files: abp.services.app.IReturnFileDto[];

  isReadonly(): boolean {
    var docDate = moment(this.docDate);
    var today = moment();    return false;//docDate.year() !== today.year() || (docDate.month() !== today.month() && (docDate.month() !== today.month() - 1 || today.date() >= 4));
  }
}