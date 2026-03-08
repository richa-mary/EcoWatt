import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/enviroment';

export interface Customer {
  customerId: number;
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
  address?: string;
  postcode: string;
  tariffId: number;
  tariff?: {
    tariffId: number;
    name: string;
    elecUnitRate: number;
    elecStandingCharge: number;
    gasUnitRate: number;
    gasStandingCharge: number;
  };
}

@Injectable({ providedIn: 'root' })
export class CustomerApi {
  private http = inject(HttpClient);
  private base = environment.apiUrl;

  getById(id: number) {
    return this.http.get<Customer>(`${this.base}/api/customers/${id}`);
  }
}
