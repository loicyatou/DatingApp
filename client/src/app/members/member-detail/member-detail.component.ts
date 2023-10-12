import { CommonModule } from '@angular/common';
import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { GalleryItem, GalleryModule, ImageItem } from 'ng-gallery';
import { TabDirective, TabsModule, TabsetComponent } from 'ngx-bootstrap/tabs';
import { TimeagoModule } from 'ngx-timeago';
import { Member } from 'src/app/_model/members';
import { MembersService } from 'src/app/_services/members.service';
import { MemberMessagesComponent } from '../member-messages/member-messages.component';
import { MessageService } from 'src/app/_services/message.service';
import { Message } from 'src/app/_model/message';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css'],

  standalone: true, //have to make this a standalone component because it wants to use ng gallery which is a standalone feautre. you cannot put standalone feautres in components that rely on urls. Thus you must now import its own dependecies since it is no longer part of the app module.

  imports: [CommonModule, TabsModule, GalleryModule, TimeagoModule, MemberMessagesComponent] //manually import common modules into the component so it can still use all the features it had access to before it became standalone. commonmodules imports all teh basic angular directives used in the html file
})
export class MemberDetailComponent implements OnInit {
  @ViewChild('memberTabs', {static: true}) memberTabs?: TabsetComponent; //#tabset references as memberTabs in html file which allows us to use it in this way here | static means that the cild component is resolved during initialisation phase before the ngOnInit lifecycle hook is called. so its essnetally available in ngOnInit to be used. this way it can be filled with data within the ngOnInit so that the messages appear when you click on certain icons. The reason this was used was that when you clicked on the message icon it wouldnt load the message tab becuase there was no data there yet and the condition in html required there to be. 

  member: Member = {} as Member; //root resovler will fill this in on initialisation --> initialises member with an empty object bvefore hand
  images: GalleryItem[] = [];
  activeTab?: TabDirective;
  messages: Message[] = [];

  constructor(private memberService: MembersService, private route: ActivatedRoute, private messageService: MessageService) { } //route: routing configuration that will match the URL it reieved to the corresponding component. look at the routing class to see the paths

  ngOnInit(): void {
    this.route.data.subscribe({
      next: data => this.member = data['member'] //route revolver will grab the data before the component is fully rendered so that the member data is available to load messages automatically when the message icon is clicked.
    })

    this.route.queryParams.subscribe({
      next: params => {
        params['tab'] && this.selectTab(params['tab'])
      }
    })

    this.getImages() //load the members photos
  }

  //activiates the messages tab when a button is clicked that is aleady showing the tabset.
  selectTab(heading: string) {
    if (this.memberTabs) {
      this.memberTabs.tabs.find(x => x.heading === heading)!.active = true;
    }
  }

  //if the messages tab is selected it will load the messages for you
  onTabActivated(data: TabDirective) {
    this.activeTab = data;
    if (this.activeTab.heading === 'Messages') {
      this.loadMessages();
    }
  }


  loadMessages() {
    if (this.member) {
      this.messageService.getMessageThread(this.member.userName).subscribe({
        next: messages => this.messages = messages
      })
    }
  }

  getImages() {
    if (!this.member) return;
    for (const photo of this.member?.photos) {
      this.images.push(new ImageItem({ src: photo.url, thumb: photo.url }));
    }
  }
}
