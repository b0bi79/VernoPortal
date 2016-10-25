import { Component, ViewEncapsulation, Input, OnInit, NgZone, ViewChild, TemplateRef, ViewContainerRef, ElementRef } from '@angular/core';
import { FilesModal } from './components/files/files.component';
import { Return } from './returns.model';
//import { MapUtils } from '../../../../app/utils/mapping-json'; 

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

  pickerOptions: Object = {
    'showDropdowns': true,
    'showWeekNumbers': true,
    'alwaysShowCalendars': true,
    'autoApply': true,
    'startDate': this.periodFilter.start,
    'endDate': this.periodFilter.end
  };

  constructor(private element: ElementRef) {
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
          self.datas = result.items.map(x => null /*MapUtils.deserialize(Return, x)*/);
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
        (doc: Return) => doc.docNum.toLowerCase().indexOf(query) >= 0 ||
          doc.supplierName.toLowerCase().indexOf(query) >= 0);
    } else {
      this.filteredDatas = this.datas;
    }
  }

  /*onFilesChanged(e, row) {
    if (e.length)
      row.status = 10;
    else
      row.status = 0;
  }*/
}
