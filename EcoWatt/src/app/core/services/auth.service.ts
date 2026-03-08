import { Injectable, signal, computed } from '@angular/core';
import { Router } from '@angular/router';
import type { AuthResponse } from '../api/auth.api';

const STORAGE_KEY = 'ecowatt_auth';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private _user = signal<AuthResponse | null>(this.loadFromStorage());

  readonly user = this._user.asReadonly();
  readonly isLoggedIn = computed(() => {
    const u = this._user();
    if (!u) return false;
    return new Date(u.expiresAt) > new Date();
  });

  constructor(private router: Router) {}

  setUser(auth: AuthResponse): void {
    this._user.set(auth);
    localStorage.setItem(STORAGE_KEY, JSON.stringify(auth));
  }

  logout(): void {
    this._user.set(null);
    localStorage.removeItem(STORAGE_KEY);
    this.router.navigate(['/login']);
  }

  getToken(): string | null {
    return this._user()?.token ?? null;
  }

  getCustomerId(): number | null {
    return this._user()?.customerId ?? null;
  }

  private loadFromStorage(): AuthResponse | null {
    try {
      const raw = localStorage.getItem(STORAGE_KEY);
      if (!raw) return null;
      const parsed: AuthResponse = JSON.parse(raw);
      if (new Date(parsed.expiresAt) <= new Date()) {
        localStorage.removeItem(STORAGE_KEY);
        return null;
      }
      return parsed;
    } catch {
      return null;
    }
  }
}
