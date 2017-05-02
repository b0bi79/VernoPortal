import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Ng2SmartTableModule } from 'ng2-smart-table';
import { NgaModule } from '../../../theme/nga.module';

import { FileUploadModule } from 'ng2-file-upload';
import { WindowViewModule } from 'ng2-window-view';
import { TooltipModule } from 'ng2-bootstrap/components/tooltip';

import { routing } from './production.routing';
import { Production } from './production.component';
import { ProductionCalculator } from './calculator';
import { ShopService } from './calculator/production.service';

import { ProductionManage } from './manage';
import { SpecEdit } from './manage/components/spec-edit';

import { ProductionSpecService } from './manage/production-spec.service';



@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    NgaModule,
    routing,
    FileUploadModule,
    TooltipModule,
    WindowViewModule,
    Ng2SmartTableModule
  ],
  declarations: [
    Production,
    ProductionCalculator,
    ProductionManage,
    SpecEdit
  ],
  providers: [
    ShopService,
    ProductionSpecService,
  ]
})
export class ProductionModule {
}
