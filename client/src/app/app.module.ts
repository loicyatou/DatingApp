import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NavComponent } from './nav/nav.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HomeComponent } from './home/home.component';
import { RegisterComponent } from './register/register.component';
import { MemberListComponent } from './members/member-list/member-list.component';
import { MemberDetailComponent } from './members/member-detail/member-detail.component';
import { ListsComponent } from './lists/lists.component';
import { MessagesComponent } from './messages/messages.component';
import { SharedModule } from './_modules/shared.module';
import { ErrorTestComponent } from './errors/error-test/error-test.component';
import { ErrorInterceptor } from './_interceptors/error.interceptor';
import { NotFoundComponent } from './errors/not-found/not-found.component';
import { ServerErrorComponent } from './errors/server-error/server-error.component';
import { MemeberCardComponent } from './member-card/memeber-card.component';
import { JwtInterceptor } from './_interceptors/jwt.interceptor';
import { MemberEditComponent } from './members/member-edit/member-edit.component';
import { LoadingInterceptor } from './_interceptors/loading.interceptor';
import { PhotoEditorComponent } from './members/photo-editor/photo-editor.component';
import { TextInputComponent } from './_forms/text-input/text-input.component';
import { DatePickerComponent } from './_forms/date-picker/date-picker.component';
import { AdminPanelComponent } from './admin/admin-panel/admin-panel.component';
import { HasRoleDirective } from './_directive/has-role.directive';
import { UserManagementComponent } from './admin/user-management/user-management.component';
import { PhotoManagementComponent } from './admin/photo-management/photo-management.component';
import { RolesModalComponent } from './modals/roles-modal/roles-modal.component';

@NgModule({//decrator used to provide metadata about the module, such as declarations, imports, providers and bootstrap comps
  declarations: [ //specifies the components, directives and pipes that belong to this module
    AppComponent, NavComponent, HomeComponent, RegisterComponent, MemberListComponent, ListsComponent, MessagesComponent, ErrorTestComponent, NotFoundComponent, ServerErrorComponent, MemeberCardComponent, MemberEditComponent, PhotoEditorComponent, TextInputComponent, DatePickerComponent, AdminPanelComponent, HasRoleDirective, UserManagementComponent, PhotoManagementComponent, RolesModalComponent
  ],
  imports: [ //defines the dependcies that module requires
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    BrowserAnimationsModule,
    FormsModule,
    ReactiveFormsModule,
    SharedModule //allows us to seperate the angula imports and external imports for insance from ngx to your angula application
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true }, //multi = false means replace the interceptors that are defualt added or true to add this interceptor to the existing onse

    { provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true },

    { provide: HTTP_INTERCEPTORS, useClass: LoadingInterceptor, multi: true }

  ], //specifies the services that are available within the module. 

  bootstrap: [AppComponent], //specfies the root component that should launch when bootstrapped
})
export class AppModule { } //entry point for application. responsible for orchestraying the different parts of your application. 
