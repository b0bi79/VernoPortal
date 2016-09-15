/// <reference path="../../../../../../app.services-typings.d.ts" />
import { Component, ViewEncapsulation, Input, OnInit, NgZone } from '@angular/core';

//import { DataTable } from 'primeng/primeng';
import * as moment from 'moment';


@Component({
  selector: 'returns-list',
  encapsulation: ViewEncapsulation.None,
  template: require('./returns-list.html'),
})
export class ReturnsList implements OnInit {
    pickerOptions: Object = {
        'showDropdowns': true,
        'showWeekNumbers': true,
        'alwaysShowCalendars': true,
        'autoApply': true,
        'startDate': moment(),
        'endDate': moment()
    };
    filter: string;
    private datas: any;//IReturnsDocument[];
    private filteredDatas: any;//abp.services.app.IPrintDocument[];

    constructor() { }

    ngOnInit() {
        this.getDatas(this.pickerOptions['startDate'], this.pickerOptions['endDate']);
    }

    getDatas(dfrom: moment.Moment, dto: moment.Moment) {
        /*var self = this;
        remoteApp.print.getList(dfrom.format("YYYY-MM-DD"), dto.format("YYYY-MM-DD")).done(result => {
            self.datas = result.items;
            self.filteredDatas = result.items;
        });*/
    }

    dateSelected(period) {
        this.getDatas(period.start, period.end);
    }

    public filterData(query: string) {
        console.log(query);
        if (query) {
            query = query.toLowerCase();
            this.filteredDatas = _.filter(this.datas,
                doc => true/* doc.imahDok.toLowerCase().indexOf(query) >= 0 ||
                    doc.srcWhId.toString() === query ||
                    doc.nomNakl.endsWith(query)*/);
        } else {
            this.filteredDatas = this.datas;
        }
    }
}
