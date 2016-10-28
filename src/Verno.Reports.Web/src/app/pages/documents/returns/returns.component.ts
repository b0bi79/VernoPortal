import { Component, ViewEncapsulation, Input, OnInit, NgZone, ViewChild, TemplateRef, ViewContainerRef, ElementRef } from '@angular/core';
import { GlobalState } from 'app/global.state';
import { FilesModal } from './components/files/files.component';
import { Return } from './returns.model';
import { MapUtils } from 'app/utils/mapping-json'; 

import * as moment from 'moment';
import app = abp.services.app;

@Component({
  selector: 'returns',
  encapsulation: ViewEncapsulation.None,
  template: require('./returns.html'),
})
export class Returns implements OnInit {
  @ViewChild('editFilesTmpl') editTmpl: TemplateRef<any>;

  private filter: string;
  private unreclaimed: boolean = false;
  private periodFilter: any = { start: moment(), end: moment() };
  private datas: Return[];
  private filteredDatas: Return[] = [];
  private selectedRow: Return;
  private needShowShops: boolean;

  pickerOptions: Object = {
    'showDropdowns': true,
    'showWeekNumbers': true,
    'alwaysShowCalendars': true,
    'autoApply': true,
    'startDate': this.periodFilter.start,
    'endDate': this.periodFilter.end
  };

  constructor(private element: ElementRef, private _state: GlobalState) {
  }

  ngOnInit() {
    this.getDatas(this.periodFilter.start, this.periodFilter.end, this.unreclaimed);
  }

  getDatas(dfrom: moment.Moment, dto: moment.Moment, unreclaimed: boolean): void {
    var self = this;
    abp.ui.setBusy(jQuery('.card', this.element.nativeElement),
    {
      blockUI: true,
      promise: app.returns.getList(dfrom.format("YYYY-MM-DD"), dto.format("YYYY-MM-DD"), unreclaimed)
        .done(result => {
          self.datas = result.items.map(x => MapUtils.deserialize(Return, x));
          if (self.datas.length) {
            var firstShop = self.datas[0].shopNum;
            self.needShowShops = self.datas.some(x => x.shopNum !== firstShop);
          }
          this.filterData(this.filter);
        })
    });
  }

  dateSelected(period): void {
    this.periodFilter = period;
    this.getDatas(period.start, period.end, this.unreclaimed);
  }

  unreclaimedChanged(value: boolean): void {
    this.unreclaimed = value;
    this.getDatas(this.periodFilter.start, this.periodFilter.end, this.unreclaimed);
  }

  filterData(query: string): void {
    this.filter = query;
    if (this.filter) {
      query = query.toLowerCase();
      this.filteredDatas = _.filter(this.datas,
        (doc: Return) =>
          doc.docNum.toLowerCase().indexOf(query) >= 0 ||
          doc.shopNum.toString() === query ||
          doc.supplierName.toLowerCase().indexOf(query) >= 0
      );
    } else {
      this.filteredDatas = this.datas;
    }
  }

  public isInRole(name: string): boolean {
    return this._state.userInRole(name);  }

  public isGranted(name: string): boolean {
    return abp.auth.isGranted(name);
  }
}
