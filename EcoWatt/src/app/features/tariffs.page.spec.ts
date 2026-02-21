import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TariffsPage } from './tariffs.page';

describe('TariffsPage', () => {
  let component: TariffsPage;
  let fixture: ComponentFixture<TariffsPage>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TariffsPage]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TariffsPage);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
