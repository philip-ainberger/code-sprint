import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CustomBadgeComponent } from './custom-badge.component';

describe('CustomBadgeComponent', () => {
  let component: CustomBadgeComponent;
  let fixture: ComponentFixture<CustomBadgeComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CustomBadgeComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(CustomBadgeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
