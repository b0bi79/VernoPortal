import { Component, ViewEncapsulation, Input, OnInit, NgZone, ElementRef } from '@angular/core';
import { WindowViewOutletComponent, WindowViewService, WindowViewLayerService } from 'ng2-window-view';
import { Subscription } from 'rxjs/Subscription';

import { CommonModule } from '@angular/common';
import { FormsModule as AngularFormsModule } from '@angular/forms';
import { NgaModule } from 'app/theme/nga.module';

import { GlobalState } from '../../../../global.state';
import { Production, ProductionDto, IdQty } from './production.model';
import { ExportToExcelService, Workbook, Worksheet, Column } from "app/theme/services";
let saver = require("file-saver");

import * as moment from 'moment';
import { ShopService } from './production.service';
import { Stickers } from './components/stickers';

@Component({
  selector: 'own-production',
  encapsulation: ViewEncapsulation.None,
  template: require('./production.html'),
  styles: ['.table th, .table td {padding: 0.65rem}'],
  providers: [ShopService, ExportToExcelService, ExportToExcelService, WindowViewService, WindowViewLayerService]
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
    state: GlobalState,
    private windowView: WindowViewService
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
    shopNum = Number(shopNum);
    if (shopNum && shopNum > 1000) {
      abp.ui.setBusy(jQuery('.card-body', this.element.nativeElement),
        {
          blockUI: true,
          promise: this.productionSvc.getList(shopNum)
            .then(result => {
              self.datas = result.items;
            })
        });
    } else {
      self.datas = [];
    }
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
    abp.ui.setBusy(jQuery('#exportExcelBtn', this.element.nativeElement),
    {
      blockUI: true,
      promise: this.productionSvc.getListExcel(this.currShopNum)
        .then(blob => {
          let fileName = 'BakeCalculator_' + moment().format('YYMMDD') + '.xlsx';
          saver.saveAs(blob, fileName);
        })
        .catch(reason => {
          abp.message.error(reason);
        })
    });
  }

  private printSticker(e): void {
    var self = this;
    this.windowView.pushBareDynamicWindow(Stickers, { imports: [CommonModule, AngularFormsModule, NgaModule] }).then(window => {
      window.position = { x: e.x - 700, y: e.y + 5 };
      window.items = self.datas.filter(x => x.toBake > 0 && x.etiketka);
      window.items.forEach(x => x.toPrintSticker = Math.round(x.toBake / 2));
      let waitResult: Subscription = window.result$.subscribe(
        ok => { // result
          if (ok) {
            window.showProgress();
            self.productionSvc.stickerFile(this.currShopNum, window.items.map(t => { return new IdQty(t.vidTovara, t.toPrintSticker) }))
              .then(blob => {
                saver.saveAs(blob, 'stickers_' + moment().format('YYYYMMDD') + '.docx');
                window.close();
              })
              .catch(error => {
                abp.message.error(error);
                window.close();
              });
          } else
            window.close();
        },
        () => { }, // error
        () => { //complete
          waitResult.unsubscribe();
        }
      );
    });
  }

  /*  private printSticker(row: ProductionDto): void {
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
    }*/

  private isGranted(name: string): boolean {
    return abp.auth.isGranted(name);
  }
}
