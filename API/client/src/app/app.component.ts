import { HttpClient } from '@angular/common/http';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subscription } from 'rxjs/internal/Subscription';
import { AccountService } from './_services/account.service';
import { User } from './_model/users';


@Component({ //a decarator used to proide metadata (below) about the component
  selector: 'app-root', //specifies what will indetify and render the component in the html-make up. here its the root
  templateUrl: './app.component.html', //specifies the file that contains the HTML template for component
  styleUrls: ['./app.component.css'] //specifies file with css styling
})


export class AppComponent implements OnInit, OnDestroy { //class acts as entry point for application and can contain other child components, services and logic specific to application. reps a top level component that holds and manages the overall strcuture and behaviour of your application

  //OnInit: On initialisation do the following. this is specified in the ngOnInit method 

  title = 'Dating App';
  users: any; //!users can be any type. this is not used often. not type safety

  //dependency inject http get inside the component. this must be a type of service
  constructor(private accountService: AccountService) { }


  ngOnInit(): void {
    this.setCurrentUser();
  }


  setCurrentUser() {
    const userString = localStorage.getItem('user'); //check local storage to see if a uer is already logged in
    if (!userString) return; //if not then they need to log in again

    const user: User = JSON.parse(userString); //if there is a user convert to JSON and store as a user type to be stored inside the custom observable array that is shared across the application context in account servic
    this.accountService.setCurrentUser(user);
  }


  //close the data stream with the observable
  ngOnDestroy(): void {
    // this.sub?.unsubscribe();
  }

}


