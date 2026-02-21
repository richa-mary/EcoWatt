import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-quote',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './quote.page.html',
  styleUrl: './quote.page.scss'
})
export class QuotePage {}
