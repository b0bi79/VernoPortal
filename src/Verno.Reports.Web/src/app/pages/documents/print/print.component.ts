import { Component, ViewEncapsulation, Input, OnInit, NgZone, ElementRef } from '@angular/core';
import { DataTableDirectives } from 'angular2-datatable/datatable'

import * as moment from 'moment';
import * as _ from 'lodash';
import app = abp.services.app;

@Component({
  selector: 'print',
  //directives: [DataTableDirectives],
  encapsulation: ViewEncapsulation.None,
  template: require('./print.html')
})
export class Print implements OnInit {
  pickerOptions: Object = {
    'showDropdowns': true,
    'showWeekNumbers': true,
    'alwaysShowCalendars': true,
    'autoApply': true,
    'startDate': moment(),
    'endDate': moment()
  };
  filter: string;
  private datas: app.IPrintDocument[];
  private filteredDatas: app.IPrintDocument[];

  constructor(private element: ElementRef) { }

  ngOnInit() {
    this.getDatas(this.pickerOptions['startDate'], this.pickerOptions['endDate']);
  }

  getDatas(dfrom: moment.Moment, dto: moment.Moment) {
    var self = this;
    abp.ui.setBusy(jQuery('.card', this.element.nativeElement),
      {
        blockUI: true,
        promise: app.print.getList(dfrom.format("YYYY-MM-DD"), dto.format("YYYY-MM-DD"))
          .done(result => {
            self.datas = result.items;
            this.filterData(this.filter);
          })
      });
  }

  public filterData(query: string) {
    console.log(query);
    if (query) {
      query = query.toLowerCase();
      this.filteredDatas = _.filter(this.datas,
        doc => doc.imahDok.toLowerCase().indexOf(query) >= 0 ||
          doc.srcWhId.toString() === query ||
          doc.nomNakl.endsWith(query));
    } else {
      this.filteredDatas = this.datas;
    }
  }

  dateSelected(period) {
    this.getDatas(period.start, period.end);
  }

  public isGranted(name: string): boolean {
    return abp.auth.isGranted(name);
  }
}
