import { Component, ViewEncapsulation, OnInit, ElementRef } from '@angular/core';
import { Router } from '@angular/router';

import { NgaModule } from 'app/theme/nga.module';

import { ProizvMagSpec, ProizvMagSpecDto } from './production-spec.model';
import { ProductionSpecService } from './production-spec.service';

@Component({
  selector: 'production-spec-list',
  encapsulation: ViewEncapsulation.None,
  template: require('./spec-list.html'),
  styles: ['.table th, .table td {padding: 0.65rem}'],
  providers: [ProductionSpecService]
})
export class ProductionManage implements OnInit {
  private datas: ProizvMagSpecDto[] = [];

  constructor(
    private element: ElementRef,
    private specSvc: ProductionSpecService,
    private router: Router
  ) {  }

  ngOnInit() {
    this.getDatas();
  }

  private getDatas(): void {
    var self = this;
    abp.ui.setBusy(jQuery('.card-body', this.element.nativeElement),
      {
        blockUI: true,
        promise: this.specSvc.getList()
          .then(result => {
            self.datas = result.items;
          })
      });
  }

  private refresh(): void {
    var self = this;
    this.getDatas();
  }

  private isGranted(name: string): boolean {
    return abp.auth.isGranted(name);
  }

  editSpec(spec: ProizvMagSpecDto) {
    this.router.navigate(["/pages", "documents", "production", "manage", "spec", spec ? spec.id : 'new']);
  }

  /*deleteSpec(spec: ProizvMagSpecDto) {
    var self = this;
    abp.message.confirm('Спецификация будет удалена.', 'Вы уверены?',
      isConfirmed => {
        if (isConfirmed) {
          this.specSvc.delete(spec.id).then(s => {
            self.datas = self.datas.filter(x => x.id !== s.id);
          });
        }
      }
    );
  }*/
}
