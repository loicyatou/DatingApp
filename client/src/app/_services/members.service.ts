import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Member } from '../_model/members';
import { map, of } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class MembersService {
  baseUrl = environment.apiUrl;
  members: Member[] = [];

  constructor(private http: HttpClient) { }

  getMembers() {
    //if youve already made a call to the API then store the current information you have about the users and return that instead
    //otherwise get the members and return them and also store them in a members array that stores the entire list of users
    if (this.members.length > 0) return of(this.members) //of: operator creates an observable that emits a single value. in this case we need the members array to be returned as an observable to the client.

    return this.http.get<Member[]>(this.baseUrl + 'users').pipe(
      map(members => {
        this.members = members
        return members;
      }))
  }


  getMember(username: string) {
    //optimised so that if youve already got the list of members just get it from the array not a request ot hte api
    const member = this.members.find(x => x.userName == username);
    if(member) return of(member);
    return this.http.get<Member>(this.baseUrl + 'users/' + username)
  }

  updateMember(member: Member) {
    //matches the put method in userscontroller. 
    return this.http.put(this.baseUrl + 'users', member).pipe(
      map(() => {
        const index = this.members.indexOf(member); //lookings for the index of the member that was just upadated in the current array we have loaded
        this.members[index] = {...this.members[index], ...member} //The spread operator (`...`) is used to create a new object that merges the existing member object at the specified index with the updated member object. This is done by spreading the properties of both objects into a new object.

        //now the member in the array that was storing all the memeers is updated without having to recall the api to refresh it
      })
    )
  }
}
