import { Component, ViewEncapsulation, Input, OnInit, NgZone, ViewChild, TemplateRef, ViewContainerRef, ElementRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { WindowViewOutletComponent, WindowViewService, WindowViewLayerService } from 'ng2-window-view';
import { Subscription } from 'rxjs/Subscription';

import { FilesModal } from './components/files/files.component';
import { Return } from './returns.model';
import { MapUtils } from 'app/utils/mapping-json';
import { ExportToExcelService, Workbook, Worksheet, Column } from "app/theme/services";
import { DataTable } from "app/theme/components"

import { PackDownload } from './components/packDownload';
import { ReturnsService } from './returns.service';

import * as moment from 'moment';
let saver = require("file-saver");

@Component({
  selector: 'returns',
  encapsulation: ViewEncapsulation.None,
  template: require('./returns.html'),
  styles: [`
    .table > tbody > tr.to-download{
      border: 1px dashed brown;
      background-color: #FFC107 !important;
    }
    select.form-control:not([size]):not([multiple]) { height: 2.2rem; }
    #shopNum-filter { width: 150px; }
  `],
  providers: [ReturnsService, ExportToExcelService, WindowViewService, WindowViewLayerService]
})
export class Returns implements OnInit {
  @ViewChild('editFilesTmpl') editTmpl: TemplateRef<any>;
  @ViewChild('mf') table: DataTable;

  private filter: string = "";
  private unreclaimed: boolean = false;
  private shopNum: number = null;
  private filial: number = null;
  private periodFilter = { start: moment(), end: moment() };
  private datas: Return[] = [];
  private selectedRow: Return;
  private needShowShops: boolean;
  private filterDelay: number = 0;
  private downloadItems: Return[];

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
    private exporter: ExportToExcelService,
    private returnsSvc: ReturnsService,
    private windowView: WindowViewService
  ) {
  }

  ngOnInit() {
    this.getDatas();
  }

  private getDatas(): void {
    var self = this;
    abp.ui.setBusy(jQuery('.card-body', this.element.nativeElement),
    {
      blockUI: true,
      promise: this.returnsSvc.getList(
            this.periodFilter.start.format("YYYY-MM-DD"),
            this.periodFilter.end.format("YYYY-MM-DD"),
            this.filter,
            Boolean(this.unreclaimed),
            Number(this.shopNum),
            Number(this.filial))
        .then(result => {
          self.datas = result.items.map(x => MapUtils.deserialize(Return, x));
          if (self.datas.length) {
            var firstShop = self.datas[0].shopNum;
            self.needShowShops = self.needShowShops || self.datas.some(x => x.shopNum !== firstShop);
          }
        })
    });
  }

  private dateSelected(period): void {
    this.periodFilter = period;
    this.getDatas();
  }

  private unreclaimedChanged(value: boolean): void {
    this.unreclaimed = value;
    this.getDatas();
  }

  private filterData(query: string): void {
    this.filter = query;
    this.filterWithDelay();
  }

  private filterShopNum(shopNum: number): void {
    this.shopNum = shopNum;
    this.filterWithDelay();
  }

  private filterFilial(filial: number): void {
    this.filial = filial;
    this.getDatas();
  }

  private filterWithDelay(): void {
    var self = this;
    this.filterDelay++;
    setTimeout(() => {
      if (self.filterDelay === 1) {
        this.getDatas();
      }
      self.filterDelay--;
    }, 1000);
  }

  private exportExcel(): void {
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
            <Column>{ header: "Линия", eval: r => r.liniahTip, width: 20 }
          ]
        }
      ]
    };
    this.exporter.export(wb, "returns.xlsx");
  }

  private packDownload(e): void {
    var self = this;
    this.downloadItems = [];
    this.windowView.pushBareDynamicWindow(PackDownload, { imports: [CommonModule] }).then(window => {
      window.position = { x: e.x-300, y: e.y+50 };
      window.items = self.downloadItems;
      let waitResult: Subscription = window.result$.subscribe(
        ok => { // result
          if (ok) {
            window.showProgress();
            self.returnsSvc.downloadPack(self.downloadItems.map(x => x.returnId))
              .then(blob => {
                saver.saveAs(blob, 'returnspack.zip');
                window.close();
              })
              .catch(error => { window.close(); });
          } else
            window.close();
        }, 
        () => {}, // error
        () => { //complete
          self.downloadItems = undefined;
          waitResult.unsubscribe();
        }
      );
    });
  }

  private get isDownloadMode(): boolean {
    return !!this.downloadItems;
  }

  private onRowClick(row: Return): void {
    if (this.isDownloadMode) {
      var idx = this.downloadItems.indexOf(row);
      if (idx >= 0)
        this.downloadItems.splice(idx, 1);
      else {
        if (row.status < 10)
          abp.message.warn("К возврату не прикреплено ни одного файла.", "Нет файлов");
        else
          this.downloadItems.push(row);
      }
    }
  }

  private isInDownloadItems(row): boolean {
    return this.isDownloadMode && this.downloadItems.indexOf(row) >= 0;
  }

  private isGranted(name: string): boolean {
    return abp.auth.isGranted(name);
  }
}
