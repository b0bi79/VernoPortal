import { Component, ViewEncapsulation, Input, OnInit, NgZone, ViewChild, TemplateRef, ViewContainerRef, ElementRef } from '@angular/core';
//import { WindowViewOutletComponent, WindowViewService, WindowViewLayerService } from 'ng2-window-view';

import { FilesModal } from './components/files/files.component';
import { Return } from './returns.model';
import { MapUtils } from 'app/utils/mapping-json';
import { ExportToExcelService, Workbook, Worksheet, Column } from "app/theme/services";
import { DataTable } from "app/theme/components"

//import { PackDownload } from './components/packDownload';

import * as moment from 'moment';
import app = abp.services.app;

@Component({
  selector: 'returns',
  encapsulation: ViewEncapsulation.None,
  template: require('./returns.html'),
  providers: [ExportToExcelService/*, WindowViewService, WindowViewLayerService*/]
})
export class Returns implements OnInit {
  @ViewChild('editFilesTmpl') editTmpl: TemplateRef<any>;
  @ViewChild('mf') table: DataTable;

  private filter: string;
  private unreclaimed: boolean = false;
  private periodFilter: any = { start: moment(), end: moment() };
  private datas: Return[] = [];
  private selectedRow: Return;
  private needShowShops: boolean;
  private filterDelay: number = 0;

  pickerOptions: Object = {
    'showDropdowns': true,
    'showWeekNumbers': true,
    'alwaysShowCalendars': true,
    'autoApply': true,
    'startDate': this.periodFilter.start,
    'endDate': this.periodFilter.end
  };

  constructor(
    private element: ElementRef,
    private exporter: ExportToExcelService/*,
    private windowView: WindowViewService*/
  ) {
  }

  ngOnInit() {
    this.getDatas(this.periodFilter.start, this.periodFilter.end, "", this.unreclaimed);
  }

  getDatas(dfrom: moment.Moment, dto: moment.Moment, filter: string, unreclaimed: boolean): void {
    var self = this;
    abp.ui.setBusy(jQuery('.card', this.element.nativeElement),
    {
      blockUI: true,
      promise: app.returns.getList(dfrom.format("YYYY-MM-DD"), dto.format("YYYY-MM-DD"), filter, unreclaimed)
        .done(result => {
          self.datas = result.items.map(x => MapUtils.deserialize(Return, x));
          if (self.datas.length) {
            var firstShop = self.datas[0].shopNum;
            self.needShowShops = self.datas.some(x => x.shopNum !== firstShop);
          }
          //this.doFilterData(this.filter);
        })
    });
  }

  dateSelected(period): void {
    this.periodFilter = period;
    this.getDatas(period.start, period.end, this.filter, this.unreclaimed);
  }

  unreclaimedChanged(value: boolean): void {
    this.unreclaimed = value;
    this.getDatas(this.periodFilter.start, this.periodFilter.end, this.filter, this.unreclaimed);
  }

  filterData(query: string): void {
    var self = this;
    this.filter = query;
    this.filterDelay++;
    setTimeout(() => {
      if (self.filterDelay == 1) {
        this.getDatas(this.periodFilter.start, this.periodFilter.end, this.filter, this.unreclaimed);
        //this.doFilterData(query);
      }
      self.filterDelay--;
    }, 1000);
  }

  exportExcel(): void {
    var wb = <Workbook>{
      sheets: [
        <Worksheet>{
          name: "Возвраты",
          data: this.datas,
          columns: [
            <Column>{ header: "Магазин", eval: r => r.shopNum, width: 10 },
            <Column>{ header: "Дата накл", eval: r => new Date(r.docDate), width: 13 },
            <Column>{ header: "№ накладной", eval: r => r.docNum, width: 13 },
            <Column>{ header: "Поставщик", eval: r => r.supplierName, width: 40 },
            <Column>{ header: "Сумма", eval: r => r.summ, width: 15 },
            <Column>{ header: "Линия", eval: r => r.liniahTip, width: 20 },
          ]
        }
      ]
    };
    this.exporter.export(wb, "returns.xlsx");
  }

  public packDownload(): void {
    //this.windowView.pushWindow(PackDownload);
    /*this.windowView.pushBareDynamicWindow(PackDownload).then(window => {
      window.position = { x: 600, y: 400 };
    });*/
  }

  public isGranted(name: string): boolean {
    return abp.auth.isGranted(name);
  }
}
