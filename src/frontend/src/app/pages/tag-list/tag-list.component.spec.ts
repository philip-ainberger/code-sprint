import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TagListPage } from './tag-list.component';

describe('TagListComponent', () => {
  let component: TagListPage;
  let fixture: ComponentFixture<TagListPage>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TagListPage]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(TagListPage);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
