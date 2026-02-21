import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TariffApi } from '../core/api/tariff.api';

@Component({
  selector: 'app-tariffs',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './tariffs.page.html',
  styleUrl: './tariffs.page.scss'
})
export class TariffsPage implements OnInit {
  private api = inject(TariffApi);

  loading = true;
  error: string | null = null;
  tariffs: any[] = [];

  ngOnInit(): void {
    console.log('TariffsPage ngOnInit fired ✅');

    this.api.getAll().subscribe({
      next: (data) => {
        console.log('Tariffs response ✅', data);
        this.tariffs = Array.isArray(data) ? data : [];
        this.loading = false;
      },
      error: (err) => {
        console.log('Tariffs error ❌', err);
        this.error = 'Could not load tariffs.';
        this.loading = false;
      }
    });
  }

  selectTariff(t: any) {
    console.log('Selected tariff:', t);
  }
}
