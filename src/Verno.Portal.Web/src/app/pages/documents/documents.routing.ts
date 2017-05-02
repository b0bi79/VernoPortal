import { Routes, RouterModule } from '@angular/router';

import { Documents } from './documents.component';
import { Print } from './print';
import { Returns } from './returns';

// noinspection TypeScriptValidateTypes
const routes: Routes = [
    {
        path: '',
        component: Documents,
        children: [
            { path: 'print-list', component: Print },
            { path: 'returns', component: Returns },
            { path: 'production', loadChildren: 'app/pages/documents/production/production.module#ProductionModule' },
        ]
    }
];

export const routing = RouterModule.forChild(routes);
