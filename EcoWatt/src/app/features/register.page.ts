import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink, ActivatedRoute } from '@angular/router';
import { AuthApi } from '../core/api/auth.api';
import { AuthService } from '../core/services/auth.service';
import { TariffApi, Tariff } from '../core/api/tariff.api';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './register.page.html',
  styleUrl: './register.page.scss'
})
export class RegisterPage implements OnInit {
  private authApi = inject(AuthApi);
  private authService = inject(AuthService);
  private tariffApi = inject(TariffApi);
  private router = inject(Router);
  private route = inject(ActivatedRoute);

  firstName = '';
  lastName = '';
  email = '';
  password = '';
  confirmPassword = '';
  phone = '';
  address = '';
  postcode = '';
  selectedTariffId = 1;

  tariffs: Tariff[] = [];
  loading = false;
  error: string | null = null;

  ngOnInit() {
    this.tariffApi.getAll().subscribe({ next: t => this.tariffs = t });

    // Pre-fill from query params (from quote page)
    this.route.queryParams.subscribe(p => {
      if (p['tariffId']) this.selectedTariffId = +p['tariffId'];
      if (p['postcode']) this.postcode = p['postcode'];
    });

    // Redirect if already logged in
    if (this.authService.isLoggedIn()) {
      this.router.navigate(['/dashboard']);
    }
  }

  register() {
    if (!this.firstName || !this.lastName || !this.email || !this.password || !this.postcode) {
      this.error = 'Please fill in all required fields.';
      return;
    }
    if (this.password !== this.confirmPassword) {
      this.error = 'Passwords do not match.';
      return;
    }
    if (this.password.length < 8) {
      this.error = 'Password must be at least 8 characters.';
      return;
    }

    this.loading = true;
    this.error = null;

    this.authApi.register({
      firstName: this.firstName,
      lastName: this.lastName,
      email: this.email,
      password: this.password,
      phone: this.phone,
      address: this.address,
      postcode: this.postcode,
      tariffId: this.selectedTariffId
    }).subscribe({
      next: (res) => {
        this.authService.setUser(res);
        this.router.navigate(['/dashboard']);
      },
      error: (err) => {
        if (err.status === 409) {
          this.error = 'An account with this email already exists.';
        } else {
          this.error = err.error?.message || 'Registration failed. Please try again.';
        }
        this.loading = false;
      }
    });
  }
}
