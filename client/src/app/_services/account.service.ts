import { HttpBackend, HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, map } from 'rxjs';
import { User } from '../_model/users';
import { environment } from 'src/environments/environment';

//Purpose of class: This is an angula service that abstracts login logic so that it can be reusuable. The logic that it abstracts is making the http request from the client (angula nav bar) to the server(api)

//marks a class as available for dep injection
@Injectable({ providedIn: 'root' }) //providedIn auto injects the class into any component that needs it without manual config in the app.module file. here its injected into the root 

export class AccountService {

  baseUrl = environment.apiUrl; //taken from your enviroment file. make sure you import the one you want at the top

  private currentUserSource = new BehaviorSubject<User | null>(null); //behaviour subject is commonly used in scenarios where you want to share and propagate the current state or value to multiple parts of your application. mostly used when you want to keep track of the current user and share that information across difference components or services.

  currentUserSource$ = this.currentUserSource.asObservable(); //converts it to an observable that you can subscribe to across the application context and receive the most recent value emitted by the behaviour subject. Using a next() method with this you can update the current value and notify all subscribers. 

  constructor(private http: HttpClient) { }


  login(model: any) {
    return this.http.post<User>(this.baseUrl + 'account/login', model).pipe( //pipe chains numerous operators
      map((response: User) => { //map takes an emitted value and transforms it if not passes it along chain
        const user = response;
        if (user) { //callback function will check if user is != null and transforms it into a json string to be stored in browser local storage so that it can persist when a user logs in
          this.setCurrentUser(user);
        }
      })
    )

    //it will return an observable of the response in JSON format i.e. what the API wants to respond in response to the post request
  }

  register(model: any) {
    return this.http.post<User>(this.baseUrl + 'account/register', model).pipe( //request calls account/register controller and adds user to the DB
      map(user => { //response contains JSON of user created
        if (user) { 
          this.setCurrentUser(user)
        }
      })
    )
  }

  setCurrentUser(user: User) { //setter so that this can be used in other parts of the application when the state of the user needs to be changed.
    user.roles = []; //check there roles and see if there are multiple
    const roles = this.getDecodedToken(user.token).role; //gets the roles of the user from the token
    Array.isArray(roles) ? user.roles = roles : user.roles.push(roles); //if there are multiple roles to add then it will push those roles onto the roles array. if theres only one it just adds it to it
    localStorage.setItem('user', JSON.stringify(user)); //stores user into the local storage so that they remain logged in
    this.currentUserSource.next(user);  //stores user username and token so its available until user logs out
  }

  logout() {
    localStorage.removeItem('user')
    this.currentUserSource.next(null);
  }

  getDecodedToken(token: string){ //takes the token so that it can get specific informatuon out of it. here we want the second section of the token which shows the roles of the user
    return JSON.parse //grabs the json and converts it into an array
    (atob(token.split('.')[1])) //atob decodes the base64 encoded payload i.e. the token so that we know the info
  }

  //returns only the second section that we need
}
