import { HttpClient } from '@angular/common/http';
import { Injectable, OnInit } from '@angular/core';
import { environment } from 'src/environments/environment';
import { User } from '../_model/users';

@Injectable({
  providedIn: 'root'
})
export class AdminService implements OnInit {

  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }
  ngOnInit(): void {
  }

  getUsersWithRoles() {
    return this.http.get<User[]>(this.baseUrl + 'admin/users-with-roles')
  }

  updateUserRoles(username: string, roles: string) {
    return this.http.post<string[]>(this.baseUrl + 'admin/edit-roles/' 
    + username + '?roles=' + roles, {}); //need to add an empty object into post requests
  }
}
