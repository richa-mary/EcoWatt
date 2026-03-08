import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { QuoteApi, QuoteResult } from '../core/api/quote.api';

@Component({
  selector: 'app-quote',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './quote.page.html',
  styleUrl: './quote.page.scss'
})
export class QuotePage {
  private api = inject(QuoteApi);
  private router = inject(Router);

  postcode = '';
  monthlyElec = 200;
  monthlyGas = 120;

  loading = false;
  error: string | null = null;
  results: QuoteResult[] = [];
  showResults = false;
  bestIdx = -1;

  getQuote() {
    const pc = this.postcode.trim();
    if (!pc) { this.error = 'Please enter your postcode.'; return; }
    if (this.monthlyElec < 0 || this.monthlyGas < 0) {
      this.error = 'Usage values must be positive.';
      return;
    }
    this.loading = true;
    this.error = null;
    this.showResults = false;

    this.api.getEstimate(pc, this.monthlyElec, this.monthlyGas).subscribe({
      next: (data) => {
        this.results = Array.isArray(data) ? data : (data as any)?.value ?? [];
        // compute best index once here instead of on every CD cycle
        this.bestIdx = this.results.length
          ? this.results.reduce((bi, r, i) =>
              r.estimatedMonthlyAmount < this.results[bi].estimatedMonthlyAmount ? i : bi, 0)
          : -1;
        this.showResults = true;
        this.loading = false;
        setTimeout(() => {
          document.getElementById('results-section')?.scrollIntoView({ behavior: 'smooth' });
        }, 100);
      },
      error: () => {
        this.error = 'Could not fetch quotes. Please check your API is running.';
        this.loading = false;
      }
    });
  }

  selectTariff(tariffId: number) {
    this.router.navigate(['/register'], {
      queryParams: { tariffId, postcode: this.postcode.trim() }
    });
  }
}

