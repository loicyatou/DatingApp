import { CommonModule } from '@angular/common';
import { Component, Input, ViewChild } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { GalleryModule } from 'ng-gallery';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { TimeagoModule } from 'ngx-timeago';
import { Message } from 'src/app/_model/message';
import { MessageService } from 'src/app/_services/message.service';

@Component({
  selector: 'app-member-messages',
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css'],

  standalone: true,
  imports: [CommonModule, TimeagoModule, FormsModule]
})
export class MemberMessagesComponent {

@ViewChild('messageForm') messageForm?: NgForm
@Input() messages: Message[] = []
@Input() username?: string;
messageContent = '';

  constructor(private messageService: MessageService) { }

  ngOnInit(): void {
  }

  sendMessage(){
    console.log('Am I getting hit');
    if(!this.username) return;
    console.log('Am I getting hit again');
    this.messageService.sendMessage(this.username,this.messageContent).subscribe({ //se
      next: message =>{ 
        this.messages.push(message)
        this.messageForm?.reset();
      }
    })
  }

}
