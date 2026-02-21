import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable({ providedIn: 'root' })
export class TariffApi {
  private http = inject(HttpClient);

  private baseUrl = 'http://localhost:5171'; // <-- change to your backend URL/port

  getAll() {
    return this.http.get<any[]>(`${this.baseUrl}/api/tariffs`);
  }
}
