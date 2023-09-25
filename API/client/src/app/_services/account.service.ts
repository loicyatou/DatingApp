import { HttpBackend, HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, map } from 'rxjs';
import { User } from '../_model/users';

//Purpose of class: This is an angula service that abstracts login logic so that it can be reusuable. The logic that it abstracts is making the http request from the client (angula nav bar) to the server(api)

//marks a class as available for dep injection
@Injectable({ providedIn: 'root' }) //providedIn auto injects the class into any component that needs it without manual config in the app.module file. here its injected into the root 

export class AccountService {

  baseUrl = 'https://localhost:5001/api/';
  private currentUserSource = new BehaviorSubject<User | null>(null); //behaviour subject is commonly used in scenarios where you want to share and propagate the current state or value to multiple parts of your application. mostly used when you want to keep track of the current user and share that information across difference components or services.

  currentUserSource$ = this.currentUserSource.asObservable(); //converts it to an observable that you can subscribe to across the application context and receive the most recent value emitted by the behaviour subject. Using a next() method with this you can update the current value and notify all subscribers. 

  constructor(private http: HttpClient) { }


  login(model: any) {
    return this.http.post<User>(this.baseUrl + 'account/login', model).pipe( //pipe chains numerous operators
      map((response: User) => { //map takes an emitted value and transforms it if not passes it along chain
        const user = response;
        if (user) { //callback function will check if user is != null and transforms it into a json string to be stored in browser local storage so that it can persist when a user logs in
          localStorage.setItem('user', JSON.stringify(user))
          this.currentUserSource.next(user);
        }
      })
    )

    //it will return an observable of the response in JSON format i.e. what the API wants to respond in response to the post request
  }

  register(model: any) {
    return this.http.post<User>(this.baseUrl + 'account/register', model).pipe( //request calls account/register controller and adds user to the DB
      map(user => { //response contains JSON of user created
        if (user) { //if it was successful...
          localStorage.setItem('user', JSON.stringify(user)); //stores user into the local storage so that they remain logged in
          this.currentUserSource.next(user); //stores user username and token so its available until user logs out
        }
      })
    )
  }

  setCurrentUser(user: User) { //setter so that this can be used in other parts of the application when the state of the user needs to be changed.
    this.currentUserSource.next(user);
  }

  logout() {
    localStorage.removeItem('user')
    this.currentUserSource.next(null);
  }
}
