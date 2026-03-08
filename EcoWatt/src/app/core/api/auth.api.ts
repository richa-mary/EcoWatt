import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/enviroment';

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
  phone?: string;
  address?: string;
  postcode: string;
  tariffId: number;
}

export interface AuthResponse {
  token: string;
  customerId: number;
  firstName: string;
  lastName: string;
  email: string;
  tariffId: number;
  expiresAt: string;
}

@Injectable({ providedIn: 'root' })
export class AuthApi {
  private http = inject(HttpClient);
  private base = environment.apiUrl;

  login(req: LoginRequest) {
    return this.http.post<AuthResponse>(`${this.base}/api/auth/login`, req);
  }

  register(req: RegisterRequest) {
    return this.http.post<AuthResponse>(`${this.base}/api/auth/register`, req);
  }
}
