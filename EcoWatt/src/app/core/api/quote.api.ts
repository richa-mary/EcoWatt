import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/enviroment';

export interface QuoteResult {
  estimatedMonthlyAmount: number;
  estimatedAnnualAmount: number;
  tariffId: number;
  tariffName: string;
}

@Injectable({ providedIn: 'root' })
export class QuoteApi {
  private http = inject(HttpClient);
  private base = environment.apiUrl;

  getEstimate(postcode: string, monthlyElectricity: number, monthlyGas: number) {
    return this.http.get<QuoteResult[]>(`${this.base}/api/quote/estimate`, {
      params: {
        postcode,
        monthlyElectricity: monthlyElectricity.toString(),
        monthlyGas: monthlyGas.toString()
      }
    });
  }
}
