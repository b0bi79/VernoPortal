import { Routes, RouterModule } from '@angular/router';

import { Production } from './production.component';
import { ProductionCalculator } from './calculator';
import { ProductionManage } from './manage';
import { SpecEdit } from './manage/components/spec-edit';

// noinspection TypeScriptValidateTypes
const routes: Routes = [
  {
    path: '',
    component: Production,
    children: [
      { path: '', redirectTo: 'calculator', pathMatch: 'full' },
      { path: 'calculator', component: ProductionCalculator },
      { path: 'manage', component: ProductionManage },
      { path: 'manage/spec/new', component: SpecEdit },
      { path: 'manage/spec/:id', component: SpecEdit },
    ]
  }
];

export const routing = RouterModule.forChild(routes);
