import { Routes, RouterModule }  from '@angular/router';

import { Reports } from './reports.component';

// noinspection TypeScriptValidateTypes
const routes: Routes = [
  {
    path: '',
    component: Reports,
    children: [
    ]
  }
];

export const routing = RouterModule.forChild(routes);
