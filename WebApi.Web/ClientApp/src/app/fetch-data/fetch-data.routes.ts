import { Routes } from '@angular/router';

import { FetchDataComponent } from './fetch-data.component';
import { AuthGuard } from 'src/helpers/auth.guard';

export const FetchDataRoutes: Routes = [
    { path: 'fetch-data', component: FetchDataComponent, canActivate: [AuthGuard] }
];
