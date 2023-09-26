import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent {

  users: any;

  constructor(private http: HttpClient){
    this.getUsers();
  }

  registerMode = false;

  registerToggle(){
    this.registerMode = !this.registerMode; //toggle for when user clicks on register button to show/hide reg form 
  }

  getUsers() {
    //http get requests return observables which is a data stream that can be subscribed to allowing mutliple vlaues to be emitted over time.
    this.http.get('https://localhost:5001/api/users').subscribe({
      next: response => this.users = response,  //specifies what will happen after the request is retrieved
      error: error => console.log(error),//what we want to do if an error arise
      complete: () => console.log('Request has completed') //what we want to do once the request is completed  
    })
  }

  cancelRegisterMode(event: boolean){
    this.registerMode = event; //
  }
}
