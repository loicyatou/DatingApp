import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

@Component({ //a decarator used to proide metadata (below) about the component
  selector: 'app-root', //specifies what will indetify and render the component in the html-make up. here its the root
  templateUrl: './app.component.html', //specifies the file that contains the HTML template for component
  styleUrls: ['./app.component.css'] //specifies file with css styling
})


export class AppComponent implements OnInit { //class acts as entry point for application and can contain other child components, services and logic specific to application. reps a top level component that holds and manages the overall strcuture and behaviour of your application

  //OnInit: On initialisation do the following. this is specified in the ngOnInit method 

  title = 'Dating App';
  users: any; //!users can be any type. this is not used often. not type safety


  //dependency inject http get inside the component. this must be a type of service
  constructor(private http: HttpClient) { }


  ngOnInit(): void {
    //http get requests return observables which is a data stream that can be subscribed to allowing mutliple vlaues to be emitte over time.
    //
    this.http.get('https://localhost:5001/api/users').subscribe({
      next: response =>  this.users = response,  //specifies what will happen after the request is retrieved
      error: error => console.log(error),//what we want to do if an error arise
      complete: () => console.log('Request has completed') //what we want to do once the request is completed  
    })
  }


}


