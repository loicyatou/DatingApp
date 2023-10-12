import { Component, OnInit } from '@angular/core';
import { Message } from '../_model/message';
import { Pagination } from '../_model/pagination';
import { MessageService } from '../_services/message.service';

@Component({
  selector: 'app-messages',
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.css']
})
export class MessagesComponent implements OnInit {
  messages?: Message[];
  pagination?: Pagination;
  container = 'Unread';
  pageNumber = 1;
  pageSize = 5;
  loading = false; //a flag used in the html combined to hide messages untill youve loaded new one so you dont get residue from memory cahce showing up

  ngOnInit(): void {
    this.loadMessages();
  }

  constructor(private messageService: MessageService) {
  }

  loadMessages() {
    this.loading = true; 
    this.messageService.getMessages(this.pageNumber, this.pageSize, this.container).subscribe({
      next: response => {
        if (response) {
          this.messages = response.result;
          this.pagination = response.pagination;
          this.loading = false;
        }
      }
    })
  }

  deleteMessage(id: number){
    this.messageService.deleteMessage(id).subscribe({
      //goes to api then removes from record then removes from array here so it doesnt need to make a new call to the db till the next request
      next: () => this.messages?.splice(this.messages.findIndex( m => m.id === id),1) //splice removes an elemenet from the array
    })
  }

  pageChanged(event: any) {
    if (this.pageNumber !== event.page) {
      this.pageNumber = event.page; //changes the pageNumber to the page number set on the pagination component provided by ngx bootstrap
      this.loadMessages();
    }
  }
}
