import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { ToastrModule } from 'ngx-toastr';



@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    BsDropdownModule.forRoot(), //provides two way binding --> Exports the required infrastructure and directives for reactive forms, making them available for import by NgModules that import this module.
    ToastrModule.forRoot({
      positionClass: 'toast-bottom-right'
    })
  ],
  exports:[
    BsDropdownModule,
    ToastrModule
  ]
})
export class SharedModule { }
