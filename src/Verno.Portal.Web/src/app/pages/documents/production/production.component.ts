import { Component, ViewEncapsulation, Input, OnInit, NgZone, ElementRef } from '@angular/core';
import { CommonModule } from '@angular/common';

import { GlobalState } from '../../../global.state';
import { Production, ProductionDto } from './production.model';
import { ExportToExcelService, Workbook, Worksheet, Column } from "app/theme/services";
let saver = require("file-saver");

import * as moment from 'moment';
import { ShopService } from './production.service';

@Component({
  selector: 'own-production',
  encapsulation: ViewEncapsulation.None,
  template: require('./production.html'),
  styles: ['.table th, .table td {padding: 0.65rem}'],
  providers: [ShopService, ExportToExcelService]
})
export class ProductionCalculator implements OnInit {
  private datas: ProductionDto[] = [];
  private userShopNum: number;
  private currShopNum: number;
  private filterDelay: number = 0;
  private date1: moment.Moment = moment().subtract(7, 'days');
  private today: moment.Moment = moment();

  constructor(
    private element: ElementRef,
    private exporter: ExportToExcelService,
    private productionSvc: ShopService,
    state: GlobalState
  ) {
    this.userShopNum = Number(state.user.shopNum);
    this.currShopNum = this.userShopNum;
  }

  ngOnInit() {
    if (this.currShopNum)
      this.getDatas(this.currShopNum);
  }

  private getDatas(shopNum: number): void {
    var self = this;
    abp.ui.setBusy(jQuery('.card-body', this.element.nativeElement),
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
    this.currShopNum = shopNum;
    this.filterDelay++;
    setTimeout(() => {
      if (self.filterDelay === 1) {
        this.getDatas(shopNum);
      }
      self.filterDelay--;
    }, 1000);
  }

  private exportExcel(): void {
   /* var wb = <Workbook>{
      sheets: [
        <Worksheet>{
          name: "Калькулятор выпечки",
          data: this.datas,
          columns: [
            <Column>{ header: "Код заказа", eval: r => r.vidTovara, width: 15 },
            <Column>{ header: "Код касса", eval: r => r.shtrixKod, width: 15 },
            <Column>{ header: "Товар", eval: r => r.naimenovanie, width: 40 },
            <Column>{ header: "Норма вып., шт", eval: r => r.normativ, width: 10 },
            <Column>{ header: "Реализация шт. за " + this.date1.format("DD.MM.YYYY"), eval: r => r.realizSht, width: 10 },
            <Column>{ header: "Списания шт. за " + this.date1.format("DD.MM.YYYY"), eval: r => r.spisSht, width: 10 },
            <Column>{ header: "Коэффициент увеличения реализации, %", eval: r => r.koeff, width: 10 },
            <Column>{ header: "Конечный остаток в магазине, шт.", eval: r => r.ostSht, width: 10 },
            <Column>{ header: "ВЫПЕЧЬ, шт " + this.today.format("DD.MM.YYYY"), eval: r => r.toBake, width: 10 },
          ]
        }
      ]
    };
    this.exporter.export(wb, "returns.xlsx");*/
    this.productionSvc.getListExcel(this.currShopNum)
      .then(blob => {
        let fileName = "BakeCalculator.xlsx";
        saver.saveAs(blob, fileName);
      })
      .catch(reason => {
        abp.message.error(reason);
      });
  }

  private printSticker(row: ProductionDto): void {
    this.productionSvc.stickerFile(this.currShopNum, row.vidTovara)
      .then(blob => {
        let docxMimeType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
        if (blob.type === docxMimeType) {
          let fileName = row.naimenovanie.replace("/", "-").replace("\\", "-") + "_sticker.docx";
          saver.saveAs(blob, fileName);
        } else
          abp.message.error("Для товара не добавлен шаблон этикетки", "Не найдена этикетка!");
      })
      .catch(reason => {
        if (reason.status === 404)
          abp.message.error("Для товара не добавлен шаблон этикетки", "Не найдена этикетка!");
        else
          abp.message.error(reason);
      });
  }

  private isGranted(name: string): boolean {
    return abp.auth.isGranted(name);
  }
}
