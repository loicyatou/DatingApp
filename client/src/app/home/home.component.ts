
import { Component } from '@angular/core';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent {

  users: any;

  constructor(){
  }

  registerMode = false;

  registerToggle(){
    this.registerMode = !this.registerMode; //toggle for when user clicks on register button to show/hide reg form 
  }

  cancelRegisterMode(event: boolean){
    this.registerMode = event; //
  }
}
