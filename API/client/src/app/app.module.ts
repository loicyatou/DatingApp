import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import {HttpClientModule} from '@angular/common/http';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

@NgModule({//decrator used to provide metadata about the module, such as declarations, imports, providers and bootstrap comps
  declarations: [ //specifies the components, directives and pipes that belong to this module
    AppComponent
  ],
  imports: [ //defines the dependcies that module requires
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    BrowserAnimationsModule
  ],
  providers: [], //specifies the services that are available within the module. 
  bootstrap: [AppComponent] //specfies the root component that should launch when bootstrapped
})
export class AppModule { } //entry point for application. responsible for orchestraying the different parts of your application. 
