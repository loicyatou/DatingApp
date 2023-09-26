import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-server-error',
  templateUrl: './server-error.component.html',
  styleUrls: ['./server-error.component.css']
})
export class ServerErrorComponent {
  error: any;

  constructor(private router: Router){
    const navigation = this.router.getCurrentNavigation(); //gets the current view thats intiailised when the browwser nagiates to this page/component
    this.error = navigation?.extras?.state?.['error']; //checks the state of the navigation to see if there is an error. Since there may not be error there possibility that a null is returnd must be considered

    //the name of the state is the name that was provided in the interceptor class where this error is caught in the first place
  }
}
