import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';
import { AbstractControl, FormBuilder, FormControl, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {

  //review best practice with jeremy in this regard 
  // @Input() usersFromHomeComponent: any; //decorateor that allows you to pass metadata from a parent componet to a child component. Here we are passing the list of users for the website from the home component --> register component

  @Output() cancelRegister = new EventEmitter(); //decorator that allows child class to communicate with parent class and pass some metadata.
  registerForm: FormGroup = new FormGroup({}) //Part of reactiveFormModule:  allow you to create clean forms without using too many directives. They also reduce the need for end-to-end testing since it’s very easy to validate your forms.
  maxDate: Date = new Date();
  validationErrors: string[] | undefined;

  constructor(private accountService: AccountService, private toastr: ToastrService,
    private fb: FormBuilder, private router: Router) { }

  initializeForm() {
    this.registerForm = this.fb.group({
      gender: ['male'],
      username: ['', Validators.required], //fomrControls: used to track the value and vliation status of an individual form control
      knownAs: ['', Validators.required],
      dateOfBirth: ['', Validators.required],
      city: ['', Validators.required],
      country: ['', Validators.required],
      password: ['', [Validators.required, Validators.minLength(4), Validators.maxLength(8)]],
      confirmPassword: ['', [Validators.required, this.matchValues('password')]]
    });

    //this listenr checks for changes in the password field and forces app to reevaluate the validaiton of confirmPassword 
    this.registerForm.controls['password'].valueChanges.subscribe({
      next: () => this.registerForm.controls['confirmPassword'].updateValueAndValidity()
    })
  }

  //this method creates a custom validator function in ngular for validating the password field against another field. i.e password with confirm password
  matchValues(matchTo: string): ValidatorFn { //Validator
    return (control: AbstractControl) => { //AControl provides some functionality to form groups such as validation. It will eiher return null if the validation passes or a validation error object if the validation fails
      return control.value == control.parent?.get(matchTo)?.value ? null : { notMatching: true }
    }
  }

  register() {
    const dob = this.getDateOnly(this.registerForm.controls['dateOfBirth'].value);
    const values = {...this.registerForm.value, dateOfBirth: dob};

    this.accountService.register(values).subscribe({
      next: () => {
        this.cancel();
      },

      error: error => {
        this.validationErrors = error
      }
    })
  }

  cancel() {
    this.cancelRegister.emit(false); //This method returns false to the register form so that it is displayed or removed from the view every time the cancel button is clicked. 
  }

  //this method returns the date with only the date in it without the time. the dateOnly object includes the time and the time zone which leads to adverse behaviour depending on the time zone of the user. 
  private getDateOnly(dob: string | undefined) {
    if (!dob) return;

    let thedob = new Date(dob);
    return new Date(thedob.setMinutes(thedob.getMinutes() - thedob.getTimezoneOffset()))
      .toISOString().slice(0, 10) //just returns the date
  }

  ngOnInit() {
    this.initializeForm();
    this.maxDate.setFullYear(this.maxDate.getFullYear() - 18) //max date is 18 years ago from the current date
  }



}
