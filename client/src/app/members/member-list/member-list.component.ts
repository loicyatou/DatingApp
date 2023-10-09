import { Component, OnInit } from '@angular/core';
import { Observable, take } from 'rxjs';
import { Member } from 'src/app/_model/members';
import { Pagination } from 'src/app/_model/pagination';
import { UserParams } from 'src/app/_model/userParams';
import { User } from 'src/app/_model/users';
import { AccountService } from 'src/app/_services/account.service';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit {

  // members$: Observable<Member[]> | undefined
  members: Member[] = [];
  pagination: Pagination | undefined;
  pageNumber = 1;
  pageSize = 5;
  userParams: UserParams | undefined;

  genderList = [{ value: 'male', display: 'Males' }, { value: 'female', display: 'Females' }];

  constructor(private memberService: MembersService) {
    this.userParams = this.memberService.getUserParams();
  }

  ngOnInit(): void {
    // this.members$ = this.memberService.getMembers();
    this.loadMembers()
  }

  loadMembers() {
    if (this.userParams) {
      this.memberService.setUserParams(this.userParams); //so that every time the list is called it remembers previous user params
      this.memberService.getMembers(this.userParams).subscribe({ //in return gets a list of users and the pagination details
        next: response => {
          if (response.result && response.pagination) {
            this.members = response.result;
            this.pagination = response.pagination;
          }
        }
      })
    }
  }

  resetFilters() {
    this.userParams = this.memberService.resetUserParams(); //sets filters back to default passing the current user 
    this.loadMembers(); //reloads memebers based off defaults
  }

  pageChanged(event: any) {
    if (this.userParams && this.userParams?.pageNumber !== event.page) {
      this.userParams.pageNumber = event.page; //changes the pageNumber to the page number set on the pagination component provided by ngx bootstrap
      this.memberService.setUserParams(this.userParams) //remember filters when the page cahnge to diff number
      this.loadMembers();
    }
  }


}
