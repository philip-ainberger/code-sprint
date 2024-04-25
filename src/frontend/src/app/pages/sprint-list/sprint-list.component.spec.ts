import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SprintListPage } from './sprint-list.component';

describe('SprintListComponent', () => {
  let component: SprintListPage;
  let fixture: ComponentFixture<SprintListPage>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SprintListPage]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(SprintListPage);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
