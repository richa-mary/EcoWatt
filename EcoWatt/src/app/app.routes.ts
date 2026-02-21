import { Routes } from '@angular/router';
import { QuotePage } from './features/quote.page';
import { LoginPage } from './features/login.page';
import { TariffsPage } from './features/tariffs.page';


export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'quote' },
  { path: 'quote', component: QuotePage },
  { path: 'login', component: LoginPage },
  { path: 'tariffs', component: TariffsPage },
];
