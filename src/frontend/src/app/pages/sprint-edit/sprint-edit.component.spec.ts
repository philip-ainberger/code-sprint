import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SprintEditPage } from './sprint-edit.component';

describe('SprintEditComponent', () => {
  let component: SprintEditPage;
  let fixture: ComponentFixture<SprintEditPage>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SprintEditPage]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(SprintEditPage);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
