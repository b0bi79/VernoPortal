import { Routes, RouterModule } from '@angular/router';
/*import { PagesRoutes } from './pages/pages.routes';
import { IdentityRoutes } from './identity/identity.routes';*/

import { NoContent } from './no-content';

export const routes: Routes = [
  { path: '', redirectTo: 'pages', pathMatch: 'full' }, //...PagesRoutes,
  { path: 'account', redirectTo: 'identity', pathMatch: 'full' }, //...IdentityRoutes,
  {
    path: '**',
    //redirectTo: '/pages/dashboard'
	component: NoContent
  },
];

export const routing = RouterModule.forRoot(routes, { useHash: true });
