import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/enviroment';

export interface Tariff {
  tariffId: number;
  name: string;
  elecUnitRate: number;
  elecStandingCharge: number;
  gasUnitRate: number;
  gasStandingCharge: number;
}

@Injectable({ providedIn: 'root' })
export class TariffApi {
  private http = inject(HttpClient);
  private base = environment.apiUrl;

  getAll() {
    return this.http.get<Tariff[]>(`${this.base}/api/tariffs`);
  }

  getById(id: number) {
    return this.http.get<Tariff>(`${this.base}/api/tariffs/${id}`);
  }
}
