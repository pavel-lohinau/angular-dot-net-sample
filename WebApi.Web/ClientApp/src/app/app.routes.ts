import { Routes, RouterModule } from '@angular/router';

import { FetchDataRoutes } from './fetch-data/fetch-data.routes';
import { CounterRoutes } from './counter/counter.routes';
import { HomeRoutes } from './home/home.routes';
import { LoginRoutes } from './login/login.routes';
import { RegisterRoutes } from './register/register.routes';

const handleWrongUrls: Routes = [
    { path: '**', redirectTo: '' }
];

export const AppRoutes: Routes = [
    ...FetchDataRoutes,
    ...CounterRoutes,
    ...HomeRoutes,
    ...LoginRoutes,
    ...RegisterRoutes,
    ...handleWrongUrls
];

export const Routing = RouterModule.forRoot(AppRoutes);
