import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/enviroment';

export interface UsageRecord {
  usageId: number;
  customerId: number;
  date: string;
  electricityReading: number;
  gasReading: number;
}

export interface AddUsageRequest {
  customerId: number;
  date: string;
  electricityReading: number;
  gasReading: number;
}

@Injectable({ providedIn: 'root' })
export class UsageApi {
  private http = inject(HttpClient);
  private base = environment.apiUrl;

  getByCustomer(customerId: number) {
    return this.http.get<UsageRecord[]>(`${this.base}/api/usage/customer/${customerId}`);
  }

  add(usage: AddUsageRequest) {
    return this.http.post<UsageRecord>(`${this.base}/api/usage`, usage);
  }
}
