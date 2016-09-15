import { Routes, RouterModule } from '@angular/router';

import { Admin } from './admin.component';
import { Users } from './components/users/users.component';

// noinspection TypeScriptValidateTypes
const routes: Routes = [
    {
        path: '',
        component: Admin,
        children: [
            { path: 'users', component: Users },
        ]
    }
];

export const routing = RouterModule.forChild(routes);
