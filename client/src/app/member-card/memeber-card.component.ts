import { Component, Input } from '@angular/core';
import { Member } from '../_model/members';

@Component({
  selector: 'app-memeber-card',
  templateUrl: './memeber-card.component.html',
  styleUrls: ['./memeber-card.component.css']
})
export class MemeberCardComponent {

  //import list of members in dating app from the parent component the members list component 
  @Input() member: Member | undefined;

}
