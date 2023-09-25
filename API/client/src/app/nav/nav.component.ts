import { Component, OnInit } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { take } from 'rxjs/internal/operators/take';
import { Observable, of } from 'rxjs';
import { User } from '../_model/users';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})

export class NavComponent implements OnInit {

  //FormsModule in app.module allows for two way binding. so when the user clicks submit the default behaviour of browswer is to send data collected to the server and reload 

  //Model is binded to the username and password so it captures the values when the submit button is clicked and stores them
  model: any = {}

  constructor(public accountService: AccountService) { } 
  //you cant see how the accountService currentUserSource$ is used here so go to the html file and you wil see how this injetion is used ithin html to retrieve information.
  
  ngOnInit(): void {
  }


  login() {
    this.accountService.login(this.model).subscribe( //passes loginmeth in ac the users login details.
      {
        next: response => { //on success it logs a response. here it is to the console 
          console.log(response);
        },
        error: error => console.log(error) //if there is error prints error 
      }
    )
  }

  logout() {
    this.accountService.logout(); //remove account details from local storage
  }




}

