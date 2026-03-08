import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { TariffApi, Tariff } from '../core/api/tariff.api';

@Component({
  selector: 'app-tariffs',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './tariffs.page.html',
  styleUrl: './tariffs.page.scss'
})
export class TariffsPage implements OnInit {
  private api = inject(TariffApi);
  private router = inject(Router);
  private route = inject(ActivatedRoute);

  loading = true;
  error: string | null = null;
  tariffs: Tariff[] = [];

  // User-adjustable usage inputs
  monthlyElec = 200;   // kWh
  monthlyGas  = 120;   // kWh
  readonly DAYS_PER_MONTH = 30;

  get totalKwh() { return (this.monthlyElec || 0) + (this.monthlyGas || 0); }

  ngOnInit(): void {
    // Pick up usage passed from the quote page (?elec=X&gas=Y)
    const p = this.route.snapshot.queryParams;
    if (p['elec'])  this.monthlyElec = +p['elec'];
    if (p['gas'])   this.monthlyGas  = +p['gas'];

    this.api.getAll().subscribe({
      next: (data) => {
        this.tariffs = Array.isArray(data) ? data : [];
        this.loading = false;
      },
      error: () => {
        this.error = 'Could not load tariffs. Please ensure the API is running.';
        this.loading = false;
      }
    });
  }

  estimatedMonthly(t: Tariff): number {
    const elec = this.monthlyElec || 0;
    const gas  = this.monthlyGas  || 0;
    const standing = (t.elecStandingCharge + t.gasStandingCharge) * this.DAYS_PER_MONTH;
    const usage = (elec * t.elecUnitRate) + (gas * t.gasUnitRate);
    return Math.round((standing + usage) * 100) / 100;
  }

  bestTariffId(): number {
    if (!this.tariffs.length) return -1;
    return this.tariffs.reduce((best, t) =>
      this.estimatedMonthly(t) < this.estimatedMonthly(best) ? t : best
    ).tariffId;
  }

  contractLength(t: Tariff): string {
    const name = t.name.toLowerCase();
    if (name.includes('variable')) return 'Rolling monthly';
    if (name.includes('24') || name.includes('2-year') || name.includes('2 year') || name.includes('2y')) return '24 months';
    if (name.includes('12') || name.includes('1-year') || name.includes('1 year') || name.includes('1y')) return '12 months';
    return '12 months';
  }

  features(t: Tariff): string[] {
    const name = t.name.toLowerCase();
    if (name.includes('variable')) {
      return ['Flexible terms', 'No commitment', '100% renewable', 'Switch anytime'];
    }
    if (name.includes('24') || name.includes('2-year') || name.includes('2 year') || name.includes('2y')) {
      return ['Fixed rates for 2 years', 'Maximum price protection', '100% renewable', 'No exit fees'];
    }
    if (name.includes('green')) {
      return ['100% green energy', 'Price guarantee', '100% renewable', 'No exit fees'];
    }
    return ['Fixed rates for 1 year', 'Price guarantee', '100% renewable', 'No exit fees'];
  }

  selectTariff(t: Tariff) {
    this.router.navigate(['/register'], { queryParams: { tariffId: t.tariffId } });
  }
}

