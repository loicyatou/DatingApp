import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Member } from '../_model/members';
import { map, of, take } from 'rxjs';
import { Photo } from '../_model/photo';
import { PaginatedResult } from '../_model/pagination';
import { UserParams } from '../_model/userParams';
import { AccountService } from './account.service';
import { User } from '../_model/users';
import { getPaginatedResults, getPaginationHeaders } from './paginationHelper';

@Injectable({
  providedIn: 'root'
})
export class MembersService {
  baseUrl = environment.apiUrl;
  members: Member[] = [];

  //to cache the results of collecting users from the table and siaplying them we cache the members needing to be returned in a map of key(query) value(result) pairs so that when a paticular query is made it searches he map first.
  memberCache = new Map();
  userParams: UserParams | undefined;
  user: User | undefined;


  constructor(private http: HttpClient, private accountService: AccountService) {
    this.accountService.currentUserSource$.pipe(take(1)).subscribe({
      next: user => {
        if (user) {
          this.userParams = new UserParams(user);
          this.user = user;
        }
      }
    })
  }

  getMember(username: string) {
    //optimised so that if youve already got the list of members just get it from the memberCacge not a request ot hte api

  //what this basicaly does is flatten the results of the membercache array so that only the details about the users remain. This was done becausetrying to get a specific member from the membercache required breaking down numerous walls since it wasnt on the surface of the objects.

    const member = [...this.memberCache.values()] //using spread operator the values of memberCache are converted into an array. This is an array of the values in the map which is the users and there details
    .reduce((arr,elem) => arr.concat(elem.result), []) //reduce basciallly performs a callback function on all elements in an array. Here it takes al the elements in the array provivided by hte spread operator and merges the properties of each array element into a new array.
    .find((member: Member) => member.userName === username);

    if(member) return of(member); //if member is already in cache return that member

    return this.http.get<Member>(this.baseUrl + 'users/' + username)
  }

  getUserParams(){
    return this.userParams;
  }

  setUserParams(params: UserParams){
    this.userParams = params;
  }

  getMembers(userParams: UserParams) { //too many parameters so just created it as an object and passed it in so the params are clean

    const response = this.memberCache.get(Object.values(userParams).join('-')); //if the userParams have already been requested before search the map and return the member list from memory

    if(response) return of(response);

    let params = getPaginationHeaders(userParams.pageNumber, userParams.pageSize);

    params = params.append('minAge', userParams.minAge);
    params = params.append('maxAge', userParams.maxAge);
    params = params.append('gender', userParams.gender);
    params = params.append('orderBy', userParams.orderBy);

    return getPaginatedResults<Member[]>(this.baseUrl + 'users',params,this.http).pipe(
      map(response => {
        this.memberCache.set(Object.values(userParams).join('-'), response); //only gets here when its a new set of userParams and stores the key as the userparams and the value as the list of members to return so that on the next call it returns the members from memory
        return response;
      })
    )
  }

  resetUserParams() {
    if (this.user) {
      this.userParams = new UserParams(this.user); //sets filters back to default passing the current user 
      return this.userParams;
  }
  return;
}

  updateMember(member: Member) {
    //matches the put method in userscontroller. 
    return this.http.put(this.baseUrl + 'users', member).pipe(
      map(() => {
        const index = this.members.indexOf(member); //lookings for the index of the member that was just upadated in the current array we have loaded
        this.members[index] = { ...this.members[index], ...member } //The spread operator (`...`) is used to create a new object that merges the existing member object at the specified index with the updated member object. This is done by spreading the properties of both objects into a new object.
        //now the member in the array that was storing all the memeers is updated without having to recall the api to refresh it
      })
    )
  }

  //pass photoid of photo selecte to be main to user controller i.e. api
  setMainPhoto(photoId: number) {
    return this.http.put(this.baseUrl + 'users/set-main-photo/' + photoId, {})
  }

  deletePhoto(photoId: number) {
    return this.http.delete(this.baseUrl + 'users/delete-photo/' + photoId)
  }

  addLike(username: string){
    return this.http.post(this.baseUrl + 'likes/' + username, {})
  }

  getLikes(predicate: string, pageNumber: number, pageSize: number){
    let params = getPaginationHeaders(pageNumber,pageSize)
    params = params.append('predicate', predicate);

    return getPaginatedResults<Member[]>(this.baseUrl + 'likes', params,this.http)
  }

}
