import { Component, ViewEncapsulation, Input, OnInit, NgZone } from '@angular/core';

import * as moment from 'moment';
import * as _ from 'lodash';

@Component({
  selector: 'print-list',
  encapsulation: ViewEncapsulation.None,
  template: require('./print-list.html')
})
export class PrintList implements OnInit {
    pickerOptions: Object = {
        'showDropdowns': true,
        'showWeekNumbers': true,
        'alwaysShowCalendars': true,
        'autoApply': true,
        'startDate': moment(),
        'endDate': moment()
    };
    filter: string;
    private datas: abp.services.app.IPrintDocument[];
    private filteredDatas: abp.services.app.IPrintDocument[];

    constructor() { }

    ngOnInit() {
        this.getDatas(this.pickerOptions['startDate'], this.pickerOptions['endDate']);
    }

    getDatas(dfrom: moment.Moment, dto: moment.Moment) {
        var self = this;
        abp.services.app.print.getList(dfrom.format("YYYY-MM-DD"), dto.format("YYYY-MM-DD")).done(result => {
            self.datas = result.items;
            self.filteredDatas = result.items;
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
