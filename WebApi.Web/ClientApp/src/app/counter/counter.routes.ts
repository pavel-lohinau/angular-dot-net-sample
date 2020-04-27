import { Routes } from '@angular/router';

import { CounterComponent } from './counter.component';
import { AuthGuard } from 'src/helpers/auth.guard';

export const CounterRoutes: Routes = [
    { path: 'counter', component: CounterComponent, canActivate: [AuthGuard] }
];
