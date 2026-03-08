import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/enviroment';

export interface BillingRecord {
  billingId: number;
  customerId: number;
  billDate: string;
  amount: number;
  status: string;
}

@Injectable({ providedIn: 'root' })
export class BillingApi {
  private http = inject(HttpClient);
  private base = environment.apiUrl;

  getByCustomer(customerId: number) {
    return this.http.get<BillingRecord[]>(`${this.base}/api/billing/customer/${customerId}`);
  }
}
