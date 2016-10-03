import { Routes, RouterModule } from '@angular/router';

import { Admin } from './admin.component';
import { Users } from './components/users/users.component';
import { UserEdit } from './components/users/components/userEdit/userEdit.component';
import { UserRolesEdit } from './components/users/components/userRoles/userRoles.component';
import { PasswordReset } from './components/users/components/resetPassword/passwordReset.component';

// noinspection TypeScriptValidateTypes
const routes: Routes = [
    {
        path: '',
        component: Admin,
        children: [
          {
            path: 'users',
            children: [
              { path: '', component: Users },
              { path: 'new', component: UserEdit },
              { path: ':id', component: UserEdit },
              { path: ':id/roles', component: UserRolesEdit },
              { path: ':id/reset-password', component: PasswordReset },
            ]

          },
        ]
    }
];

export const routing = RouterModule.forChild(routes);
