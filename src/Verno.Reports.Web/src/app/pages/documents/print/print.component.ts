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
  private periodFilter: any = { start: moment(), end: moment() };
  private filter: string;
  private datas: app.IPrintDocument[];
  private filterDelay: number = 0;

  pickerOptions: Object = {
    'showDropdowns': true,
    'showWeekNumbers': true,
    'alwaysShowCalendars': true,
    'autoApply': true,
    'startDate': this.periodFilter.start,
    'endDate': this.periodFilter.end
  };

  constructor(private element: ElementRef) { }

  ngOnInit() {
    this.getDatas(this.periodFilter.start, this.periodFilter.end, "");
  }

  getDatas(dfrom: moment.Moment, dto: moment.Moment, filter: string) {
    var self = this;
    abp.ui.setBusy(jQuery('.card', this.element.nativeElement),
      {
        blockUI: true,
        promise: app.print.getList(dfrom.format("YYYY-MM-DD"), dto.format("YYYY-MM-DD"), filter)
          .done(result => {
            self.datas = result.items;
          })
      });
  }

  public filterData(query: string) {
    var self = this;
    this.filter = query;
    this.filterDelay++;
    setTimeout(() => {
      if (self.filterDelay == 1) {
        this.getDatas(this.periodFilter.start, this.periodFilter.end, this.filter);
      }
      self.filterDelay--;
    }, 1000);
  }

  dateSelected(period) {
    this.periodFilter = period;
    this.getDatas(period.start, period.end, this.filter);
  }

  public isGranted(name: string): boolean {
    return abp.auth.isGranted(name);
  }
}
