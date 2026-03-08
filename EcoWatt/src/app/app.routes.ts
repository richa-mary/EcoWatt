import { Routes } from '@angular/router';
import { QuotePage } from './features/quote.page';
import { LoginPage } from './features/login.page';
import { TariffsPage } from './features/tariffs.page';
import { RegisterPage } from './features/register.page';
import { DashboardPage } from './features/dashboard.page';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'quote' },
  { path: 'quote', component: QuotePage },
  { path: 'login', component: LoginPage },
  { path: 'register', component: RegisterPage },
  { path: 'tariffs', component: TariffsPage },
  { path: 'dashboard', component: DashboardPage, canActivate: [authGuard] },
  { path: '**', redirectTo: 'quote' }
];

