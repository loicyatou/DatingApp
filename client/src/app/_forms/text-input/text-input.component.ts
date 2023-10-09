import { Component, Input, OnInit, Self } from '@angular/core';
import { ControlValueAccessor, FormControl, NgControl } from '@angular/forms';


//Control Value Accessor: the bridge between an anguar forms api and the native elemnt in the DOM. The Control Value Accessor is an interface that allows you to connect a custom form control (such as a custom input component) to the Angular Forms API. It defines the methods that Angular uses to read and write values to the custom control.
@Component({
  selector: 'app-text-input',
  templateUrl: './text-input.component.html',
  styleUrls: ['./text-input.component.css']
})
export class TextInputComponent implements ControlValueAccessor {

  @Input() label = '';
  @Input() type = 'text';

  //@self ensures that the ngControl param is resolved only within the current componets injector hiearchy. i.e. it will only search through the TextInputComponents and where it has been injected to resolve the ngControl dependency and provide its behaviours

  constructor(@Self() public ngControl: NgControl) {
    //By assigning `this` as the value accessor, the `TextInputComponent` is essentially saying that it will handle the communication between the form control and itself. This means that when the value of the form control changes, Angular will call the appropriate methods on the `TextInputComponent` to update its internal state.
    this.ngControl.valueAccessor = this;
  }
  writeValue(obj: any): void {
  }

  registerOnChange(fn: any): void {

  }

  registerOnTouched(fn: any): void {
  }

  //meth to get around error on using ngcontrol in html
get control(): FormControl{
  return this.ngControl.control as FormControl
}


}
