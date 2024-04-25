import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LayoutPage } from './pages/layout/layout.component';
import { SprintListPage } from './pages/sprint-list/sprint-list.component';
import { TagListPage } from './pages/tag-list/tag-list.component';
import { SprintPage } from './pages/sprint-page/sprint-page.component';
import { SprintEditPage } from './pages/sprint-edit/sprint-edit.component';
import { ComingSoonPage } from './pages/coming-soon/coming-soon.component';
import { LoginPage } from './pages/login/login.component';

const routes: Routes = [
  { 
    path: '', 
    component: LayoutPage,
    children: [
      { path: '', redirectTo: '/sprints', pathMatch: 'full' },
      { path: 'sprints', component: SprintListPage },
      { path: 'tags', component: TagListPage },
      { path: 'sprint', component: SprintPage },
      { path: 'new-sprint', component: SprintEditPage },
      { path: 'edit-sprint/:id', component: SprintEditPage },
      { path: 'sprint/:id', component: SprintPage },
      { path: 'coming-soon', component: ComingSoonPage }
    ]
  },
  {
    path: 'login',
    component: LoginPage
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }