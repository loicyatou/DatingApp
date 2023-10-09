import { Component, Input, OnInit } from '@angular/core';
import { take } from 'rxjs';
import { Member } from 'src/app/_model/members';
import { User } from 'src/app/_model/users';
import { AccountService } from 'src/app/_services/account.service';
import { environment } from 'src/environments/environment';
import { FileUploader } from 'ng2-file-upload';
import { MembersService } from 'src/app/_services/members.service';
import { Photo } from 'src/app/_model/photo';

@Component({
  selector: 'app-photo-editor',
  templateUrl: './photo-editor.component.html',
  styleUrls: ['./photo-editor.component.css']
})
export class PhotoEditorComponent implements OnInit {

  @Input() member: Member | undefined;
  uploader: FileUploader | undefined; //ng2 file upload property.
  hasBaseDropZoneOver = false; //the zone you drag and drop photo to
  baseUrl = environment.apiUrl;
  user: User | undefined;


  constructor(private accountService: AccountService, private memberService: MembersService) {
    this.accountService.currentUserSource$.pipe(take(1)).subscribe(
      {
        next: user => {
          if (user) this.user = user;
        }
      })
  }


  ngOnInit(): void {
    this.initializeUploader();
  }

  fileOverBase(e: any) {
    this.hasBaseDropZoneOver = e; //if any event listener occurs over the base then initialise hasBase... to the event. further methods will decide whether the file meets the conditions
  }

  setMainPhoto(photo: Photo) {
    this.memberService.setMainPhoto(photo.id).subscribe({
      next: () => {
        if (this.user && this.member) {
          this.user.photoUrl = photo.url; //for local storage
          this.accountService.setCurrentUser(this.user); //reset the localstorage instance with new main photo url
          this.member.photoURL = photo.url; //change cient facing member photo url

          this.member.photos.forEach(p => {
            if (p.isMain) p.isMain = false; //change the boolean tracking which of the photos is the users main pic
            if (p.id === photo.id) p.isMain = true;
          })
        }
      }
    })
  }

  initializeUploader() {
    this.uploader = new FileUploader({
      url: this.baseUrl + 'users/add-photo', //endpoint photo is uploaded to
      authToken: 'Bearer ' + this.user?.token, //authToken will use the the users token in request header for auth purposes
      isHTML5: true, //allows broswer to use html5 capabilities to handle file uplads
      allowedFileType: ['image'], //wil only allow image files to upload
      removeAfterUpload: true, //after upload images are auto removed from queue
      maxFileSize: 10 * 1023 * 1024 //max size of each imae
    });

    //event handlers 
    this.uploader.onAfterAddingFile = (file) => {
      file.withCredentials = false //triggered after a file is added to the uploader's queue. In this case, it sets the `withCredentials` property of the file to `false`, indicating that credentials should not be sent when making the request.
    }

    this.uploader.onSuccessItem = (item, response, status, headers) => { // triggered after a file is successfully uploaded. It receives the `item`, `response`, `status`, and `headers` as parameters. If a valid `response` is received, it parses it as JSON and adds the resulting `photo` object to the `photos` array of the `member` object.

      if (response) {
        const photo = JSON.parse(response);
        this.member?.photos.push(photo);

        //just a check to see if its the users first upload. If it is it is auto set to main so this will check if the photo is a main which it will only ever be when triggering this method if its the first photo ever uploaded
        if(photo.isMain && this.user && this.member){
          this.user.photoUrl = photo.url
          this.member.photoURL = photo.url;
          this.accountService.setCurrentUser(this.user);
        }
      }
    }
  }

  deletePhoto(photoId: number) {
    this.memberService.deletePhoto(photoId).subscribe({
      next: _ => {
        if (this.member) {
          this.member.photos = this.member.photos.filter(x => x.id !== photoId); //filter returns lst o all elements that match a specific condition.
          //we are reassignging the members photo[] to a list that does not include the selected photo to delete effectively getting rid of it
        }
      }
    })
  }

}
