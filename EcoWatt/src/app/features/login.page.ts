import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthApi } from '../core/api/auth.api';
import { AuthService } from '../core/services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './login.page.html',
  styleUrl: './login.page.scss'
})
export class LoginPage {
  private authApi = inject(AuthApi);
  private authService = inject(AuthService);
  private router = inject(Router);

  email = '';
  password = '';
  loading = false;
  error: string | null = null;

  login() {
    if (!this.email || !this.password) {
      this.error = 'Please enter your email and password.';
      return;
    }
    this.loading = true;
    this.error = null;

    this.authApi.login({ email: this.email.trim().toLowerCase(), password: this.password }).subscribe({
      next: (res) => {
        this.authService.setUser(res);
        this.router.navigate(['/dashboard']);
      },
      error: (err) => {
        this.error = err.status === 401
          ? 'Invalid email or password.'
          : 'Login failed. Please try again.';
        this.loading = false;
      }
    });
  }
}

