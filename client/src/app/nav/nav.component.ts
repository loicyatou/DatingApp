import { Component, OnInit } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { take } from 'rxjs/internal/operators/take';
import { Observable, of } from 'rxjs';
import { User } from '../_model/users';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})

export class NavComponent implements OnInit {

  //FormsModule in app.module allows for two way binding. so when the user clicks submit the default behaviour of browswer is to send data collected to the server and reload 

  //Model is binded to the username and password so it captures the values when the submit button is clicked and stores them
  model: any = {}

  constructor(public accountService: AccountService, private router: Router, private toastr: ToastrService) { }
  //you cant see how the accountService currentUserSource$ is used here so go to the html file and you wil see how this injetion is used ithin html to retrieve information.

  ngOnInit(): void {
  }


  login() {
    this.accountService.login(this.model).subscribe( //passes loginmeth in ac the users login details.
      {
        next: () => this.router.navigateByUrl("/members"),
      }
    )
  }

  logout() {
    this.accountService.logout(); //remove account details from local storage
    this.router.navigateByUrl("/")
  }




}

