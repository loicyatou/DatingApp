import { Component, HostListener, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { take } from 'rxjs';
import { Member } from 'src/app/_model/members';
import { User } from 'src/app/_model/users';
import { AccountService } from 'src/app/_services/account.service';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.css']
})
export class MemberEditComponent {
  //viewChild allows you to have access to the features youve directly implemented into HTML. One of those is the form. its a child of the member edit component. to access it we pass its ID to the viewChild.
@ViewChild('editForm') editForm: NgForm | undefined

//want to access the browser so a pop up wanring appears when user trys to leave edit window after making changes without saving.
@HostListener('window:beforeunload', ['$event']) unloadNotification($event: any){ //Dom listner listens for when the window is about to change. then assaigns it a handler method so that logic can be applied before event completes
  if(this.editForm?.dirty){ //if the form has been edited...
    $event.returnValue = true; //the browswer prompts a confirtmation dialog to the user asking if they ant ot eave the page. 
  }
}


  member: Member | undefined;
  user: User | null = null;

  constructor(private accountService: AccountService, private memberService: MembersService, private toastr: ToastrService ) {
    this.accountService.currentUserSource$.pipe(take(1)).subscribe(
      {
        next: user => this.user = user
      })
  }

  loadMember() {
    if(!this.user)return;
    this.memberService.getMember(this.user.userName).subscribe({
      next: member => this.member = member
    })
  }

  updateMember(){
    //this method takes the submitted changes of the member back up and passed it to the updateMember method which makes a put request to the server apis
    this.memberService.updateMember(this.editForm?.value).subscribe({
      next: _  => {
        this.toastr.success('Profile Updated Successfully'); 
        this.editForm?.reset(this.member); //changes the values of each componetn that was updated in the form and resets the form so that new changes are tracked
      }
    })
  }

  ngOnInit(): void {
    this.loadMember();
  }



}
