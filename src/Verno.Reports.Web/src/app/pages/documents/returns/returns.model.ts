import * as moment from 'moment';

export class Return implements abp.services.app.IReturnDto {
  id: number;
  shopNum: number;
  docDate: Date;
  docNum: string;
  supplierId: number;
  supplierName: string;
  summ: number;
  liniah: number;
  liniahTip: string;
  returnId: number;

  private _status: number;
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
    var today = moment();
    return docDate.year() !== today.year() || (docDate.month() !== today.month() && (docDate.month() !== today.month() - 1 || today.day() >= 4));
  }
}