import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { GalleryItem, GalleryModule, ImageItem } from 'ng-gallery';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { Member } from 'src/app/_model/members';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css'],

  standalone: true, //have to make this a standalone component because it wants to use ng gallery which is a standalone feautre. you cannot put standalone feautres in components that rely on urls. Thus you must now import its own dependecies since it is no longer part of the app module.

  imports: [CommonModule, TabsModule, GalleryModule,] //manually import common modules into the component so it can still use all the features it had access to before it became standalone. commonmodules imports all teh basic angular directives used in the html file
})
export class MemberDetailComponent implements OnInit {
  member: Member | undefined;
  images: GalleryItem[] = []

  constructor(private memberService: MembersService, private route: ActivatedRoute) { } //route: routing configuration that will match the URL it reieved to the corresponding component. look at the routing class to see the paths

  ngOnInit(): void {
    this.loadMember();
  }

  loadMember() {
    const username = this.route.snapshot.paramMap.get('username'); //gets the username from the current route
    if (!username) return;
    this.memberService.getMember(username).subscribe({ //gets the member associcated with name passed from the card component which takes it from its parent the list.
      next: member => {
        this.member = member //this memeber is used in the html to display the name of the person
        this.getImages() //load the members photos
      }
    })
  }

  getImages() {
    if (!this.member) return;
    for (const photo of this.member?.photos) {
      this.images.push(new ImageItem({src: photo.url, thumb: photo.url}));
    }
  }
}
