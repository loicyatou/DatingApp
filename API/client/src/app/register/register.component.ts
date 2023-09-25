import { Component, EventEmitter, Input, Output } from '@angular/core';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent {

  //review best practice with jeremy in this regard 
  // @Input() usersFromHomeComponent: any; //decorateor that allows you to pass metadata from a parent componet to a child component. Here we are passing the list of users for the website from the home component --> register component

  @Output() cancelRegister = new EventEmitter(); //decorator that allows child class to communicate with parent class and pass some metadata.

  constructor(private accountService: AccountService) { }

  model: any = {};

  register() {
    this.accountService.register(this.model).subscribe({
      next: () => {
        this.cancel();
      },

      error: error => console.log(error)
    })
  }


  cancel() {
    this.cancelRegister.emit(false); //This method returns false to the register form so that it is displayed or removed from the view every time the cancel button is clicked. 
  }

}
