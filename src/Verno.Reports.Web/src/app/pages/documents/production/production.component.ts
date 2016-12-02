import { Component, ViewEncapsulation, Input, OnInit, NgZone, ElementRef } from '@angular/core';
import { CommonModule } from '@angular/common';

import { GlobalState } from '../../../global.state';
import { Production, ProductionDto } from './production.model';
import { ExportToExcelService, Workbook, Worksheet, Column } from "app/theme/services";

import { ShopService } from './production.service';

@Component({
  selector: 'own-production',
  encapsulation: ViewEncapsulation.None,
  template: require('./production.html'),
  providers: [ShopService, ExportToExcelService]
})
export class ProductionCalculator implements OnInit {
  private datas: ProductionDto[] = [];
  private userShopNum: number;
  private filterDelay: number = 0;

  constructor(
    private element: ElementRef,
    private exporter: ExportToExcelService,
    private productionSvc: ShopService,
    state: GlobalState
  ) {
    this.userShopNum = Number(state.user.shopNum);
  }

  ngOnInit() {
    if (this.userShopNum)
      this.getDatas(this.userShopNum);
  }

  private getDatas(shopNum: number): void {
    var self = this;
    abp.ui.setBusy(jQuery('.card', this.element.nativeElement),
      {
        blockUI: true,
        promise: this.productionSvc.getList(shopNum)
          .then(result => {
            self.datas = result.items;
          })
      });
  }

  private refresh(shopNum: number): void {
    var self = this;
    this.filterDelay++;
    setTimeout(() => {
      if (self.filterDelay === 1) {
        this.getDatas(shopNum);
      }
      self.filterDelay--;
    }, 1000);
  }

  private exportExcel(): void {
    var wb = <Workbook>{
      sheets: [
        <Worksheet>{
          name: "Калькулятор выпечки",
          data: this.datas,
          columns: [
            <Column>{ header: "Подгруппа классификатор", eval: r => r.imahKod2, width: 40 },
            <Column>{ header: "Код св/у", eval: r => r.vidTovara, width: 15 },
            <Column>{ header: "Код касса", eval: r => r.shtrixKod, width: 15 },
            <Column>{ header: "Наименование", eval: r => r.naimenovanie, width: 40 },
            <Column>{ header: "Производитель", eval: r => r.imahPr, width: 30 },
            <Column>{ header: "Норматив", eval: r => r.normativ, width: 10 },
            <Column>{ header: "Реализация", eval: r => r.realizSht, width: 10 },
            <Column>{ header: "Списания", eval: r => r.spisSht, width: 10 },
            <Column>{ header: "Коэффициент", eval: r => r.koeff, width: 10 },
            <Column>{ header: "Выпечь сегодня", eval: r => r.toBake, width: 10 },
          ]
        }
      ]
    };
    this.exporter.export(wb, "returns.xlsx");
  }

  private isGranted(name: string): boolean {
    return abp.auth.isGranted(name);
  }
}
