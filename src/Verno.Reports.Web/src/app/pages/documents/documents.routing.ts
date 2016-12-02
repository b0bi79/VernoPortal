import { Routes, RouterModule } from '@angular/router';

import { Documents } from './documents.component';
import { Print } from './print';
import { Returns } from './returns';
import { ProductionCalculator } from './production';

// noinspection TypeScriptValidateTypes
const routes: Routes = [
    {
        path: '',
        component: Documents,
        children: [
            { path: 'print-list', component: Print },
            { path: 'returns', component: Returns },
            { path: 'production-calculator', component: ProductionCalculator },
        ]
    }
];

export const routing = RouterModule.forChild(routes);
