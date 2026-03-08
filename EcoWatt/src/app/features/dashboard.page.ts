import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../core/services/auth.service';
import { CustomerApi, Customer } from '../core/api/customer.api';
import { UsageApi, UsageRecord, AddUsageRequest } from '../core/api/usage.api';
import { BillingApi, BillingRecord } from '../core/api/billing.api';

type Tab = 'overview' | 'usage' | 'billing' | 'tariff';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './dashboard.page.html',
  styleUrl: './dashboard.page.scss'
})
export class DashboardPage implements OnInit {
  private auth = inject(AuthService);
  private customerApi = inject(CustomerApi);
  private usageApi = inject(UsageApi);
  private billingApi = inject(BillingApi);
  private router = inject(Router);

  user = this.auth.user;

  customer: Customer | null = null;
  usageRecords: UsageRecord[] = [];
  billingRecords: BillingRecord[] = [];

  activeTab: Tab = 'overview';
  loadingCustomer = true;
  loadingUsage = false;
  loadingBilling = false;

  // Add usage form
  showUsageForm = false;
  newDate = new Date().toISOString().split('T')[0];
  newElec = 0;
  newGas = 0;
  usageError: string | null = null;
  savingUsage = false;

  ngOnInit() {
    const id = this.auth.getCustomerId();
    if (!id) { this.router.navigate(['/login']); return; }

    // Safety timeout — show content after 5 s even if API is slow
    const loadTimeout = setTimeout(() => {
      if (this.loadingCustomer) { this.loadingCustomer = false; }
    }, 5000);

    this.customerApi.getById(id).subscribe({
      next: c => { clearTimeout(loadTimeout); this.customer = c; this.loadingCustomer = false; },
      error: () => { clearTimeout(loadTimeout); this.loadingCustomer = false; }
    });

    this.loadUsage(id);
    this.loadBilling(id);
  }

  loadUsage(id?: number) {
    const cid = id ?? this.auth.getCustomerId();
    if (!cid) return;
    this.loadingUsage = true;
    this.usageApi.getByCustomer(cid).subscribe({
      next: u => { this.usageRecords = u; this.loadingUsage = false; },
      error: () => this.loadingUsage = false
    });
  }

  loadBilling(id?: number) {
    const cid = id ?? this.auth.getCustomerId();
    if (!cid) return;
    this.loadingBilling = true;
    this.billingApi.getByCustomer(cid).subscribe({
      next: b => { this.billingRecords = b; this.loadingBilling = false; },
      error: () => { this.billingRecords = []; this.loadingBilling = false; }
    });
  }

  addUsage() {
    const cid = this.auth.getCustomerId();
    if (!cid) return;
    this.usageError = null;

    if (this.newElec < 0 || this.newGas < 0) {
      this.usageError = 'Readings cannot be negative.';
      return;
    }

    this.savingUsage = true;
    const req: AddUsageRequest = {
      customerId: cid,
      date: new Date(this.newDate).toISOString(),
      electricityReading: this.newElec,
      gasReading: this.newGas
    };

    this.usageApi.add(req).subscribe({
      next: () => {
        this.showUsageForm = false;
        this.savingUsage = false;
        this.loadUsage();
        this.loadBilling();
      },
      error: (err) => {
        this.usageError = err.error?.message || 'Could not save reading.';
        this.savingUsage = false;
      }
    });
  }

  logout() {
    this.auth.logout();
  }

  setTab(t: Tab) {
    this.activeTab = t;
  }

  get latestElec(): number {
    if (!this.usageRecords.length) return 0;
    return this.usageRecords[0].electricityReading;
  }

  get latestGas(): number {
    if (!this.usageRecords.length) return 0;
    return this.usageRecords[0].gasReading;
  }

  get totalBilled(): number {
    return this.billingRecords.reduce((s, b) => s + b.amount, 0);
  }

  get unpaidCount(): number {
    return this.billingRecords.filter(b => b.status === 'Unpaid').length;
  }

  trackById(_: number, r: any) { return r.usageId ?? r.billingId; }

  billStatusClass(s: string): string {
    if (s === 'Paid') return 'paid';
    if (s === 'Overdue') return 'overdue';
    return 'unpaid';
  }

  get suggestions(): { icon: string; text: string; type: 'tip' | 'warning' | 'success' }[] {
    const tips: { icon: string; text: string; type: 'tip' | 'warning' | 'success' }[] = [];
    const tariffName = (this.customer?.tariff?.name ?? '').toLowerCase();

    if (tariffName.includes('variable')) {
      tips.push({ icon: '🔒', text: 'You are on a variable tariff — switch to a Fixed plan to protect against price rises.', type: 'tip' });
    }
    if (this.unpaidCount > 0) {
      tips.push({ icon: '⚠️', text: `You have ${this.unpaidCount} unpaid bill${this.unpaidCount > 1 ? 's' : ''}. Settle them to avoid late-payment charges.`, type: 'warning' });
    }
    if (this.latestElec > 500) {
      tips.push({ icon: '💡', text: 'Your electricity usage is above average. Switching to LED lighting and A-rated appliances could cut your bill by up to 20%.', type: 'tip' });
    }
    if (this.latestGas > 300) {
      tips.push({ icon: '🏠', text: 'High gas consumption detected. Improving loft and wall insulation is one of the most cost-effective ways to reduce heating bills.', type: 'tip' });
    }
    if (this.usageRecords.length < 3) {
      tips.push({ icon: '📊', text: 'Add regular meter readings so we can give you accurate bill estimates and personalised saving tips.', type: 'tip' });
    }
    if (tariffName.includes('green') || tariffName.includes('eco')) {
      tips.push({ icon: '🌿', text: 'You are on a green energy tariff — great choice for the planet! Share with friends to earn referral credit.', type: 'success' });
    }
    if (!tips.length) {
      tips.push({ icon: '✅', text: 'Your account looks great! Keep logging readings to stay on top of your energy use.', type: 'success' });
    }
    return tips;
  }
}
