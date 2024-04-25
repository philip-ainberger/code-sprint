import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ComingSoonPage } from './coming-soon.component';

describe('ComingSoonComponent', () => {
  let component: ComingSoonPage;
  let fixture: ComponentFixture<ComingSoonPage>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ComingSoonPage]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(ComingSoonPage);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
