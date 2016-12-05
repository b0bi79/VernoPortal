import { Routes, RouterModule } from '@angular/router';

import { NoContent } from './no-content';

export const routes: Routes = [
  { path: '', redirectTo: 'pages', pathMatch: 'full' }, //...PagesRoutes,
  { path: 'account', redirectTo: 'identity' }, //...IdentityRoutes,
  {
    path: '**',
    //redirectTo: '/pages/dashboard'
	component: NoContent
  },
];

export const routing = RouterModule.forRoot(routes, { useHash: true });
