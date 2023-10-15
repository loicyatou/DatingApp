import { Component } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';

@Component({
  selector: 'app-roles-modal',
  templateUrl: './roles-modal.component.html',
  styleUrls: ['./roles-modal.component.css']
})
export class RolesModalComponent {
username = '';
availableRoles: any[] = [];
selectedRoles: any[] = [];

  //bsModalRef is used to hide the button
  constructor(public bsModalRef: BsModalRef) {
  }

  updateChecked(checkedValue: string){
    //if the role selected is not in the sleected roles array (it will return -1) then we want to add it to the selected roles array. otherwise we want to remove it
    const index = this.selectedRoles.indexOf(checkedValue);

    index !== -1 ? this.selectedRoles.splice(index,1) : this.selectedRoles.push(checkedValue); //splice goes to the index position and removes 1 item
  }

}
