import { Routes, RouterModule } from '@angular/router';

import { Account } from './identity.component'
import { UserProfile, ChangePassword } from './Components'

// noinspection TypeScriptValidateTypes
const routes: Routes = [
  {
    path: 'identity',
    component: Account,
    children: [
      {
        path: 'changePassword',
        component: ChangePassword,
      },
      {
        path: 'profile',
        component: UserProfile,
      }
    ]
  }
];

export const routing = RouterModule.forChild(routes);
