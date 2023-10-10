import { Component, Input, OnInit } from '@angular/core';
import { Member } from '../_model/members';
import { MembersService } from '../_services/members.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-memeber-card',
  templateUrl: './memeber-card.component.html',
  styleUrls: ['./memeber-card.component.css']
})
export class MemeberCardComponent implements OnInit{

  //import list of members in dating app from the parent component the members list component 
  @Input()
  member!: Member;


  constructor(private memberService: MembersService, private toastr: ToastrService) {
  }
  ngOnInit(): void {
  }

  addLike(member: Member) {
    //recieve result of post request and send a notification to the user to indicate the user they have liked in the background
    this.memberService.addLike(member.userName).subscribe({
      next: () => this.toastr.success('You have liked ' + member.knownAs)
    })
  }


}
