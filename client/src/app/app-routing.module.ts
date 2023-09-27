import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { MemberListComponent } from './members/member-list/member-list.component';
import { MemberDetailComponent } from './members/member-detail/member-detail.component';
import { ListsComponent } from './lists/lists.component';
import { MessagesComponent } from './messages/messages.component';
import { authGuard } from './_guards/auth.guard';
import { ErrorTestComponent } from './errors/error-test/error-test.component';
import { NotFoundComponent } from './errors/not-found/not-found.component';
import { ServerErrorComponent } from './errors/server-error/server-error.component';

const routes: Routes = [
  { path: '', component: HomeComponent }, //applies authguard to chec kwhether a user can activiate the route

  {
    path: '',
    runGuardsAndResolvers: 'always',
    canActivate: [authGuard],
    children: [
      { path: 'members', component: MemberListComponent, canActivate: [authGuard] },
      { path: 'members/:id', component: MemberDetailComponent, canActivate: [authGuard] },
      { path: 'lists', component: ListsComponent, canActivate: [authGuard] },
      { path: 'messages', component: MessagesComponent, canActivate: [authGuard] },
    ]
  },
  {path: 'errors',component: ErrorTestComponent},
  {path: 'not-found',component: NotFoundComponent},
  {path: 'server-error',component: ServerErrorComponent},
  { path: '**', component: NotFoundComponent, pathMatch: 'full' } //This is a default route for when a user enters a URL that doesnt resemble the other possible routes at all. ** means wildcard. 
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }