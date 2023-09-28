import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';

import { AccountService } from '../_services/account.service';
import { Observable, take, tap } from 'rxjs';

@Injectable()
export class JwtInterceptor implements HttpInterceptor {

  constructor(private accountService: AccountService) {}

  //this method is going to intercept any request made to the API that require authorisation and attach the users token to it.
  //This is so that when they log in all permissions relevant are granted from the jump. 

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    //pipe(1) will close the subscription after the first completition of the request 
    this.accountService.currentUserSource$.pipe(take(1)).subscribe({
      next: user => {
        if(user){
          request = request.clone({ //clones the request that we are currenty reading from ...
            setHeaders: {
              Authorization: `Bearer ${user.token}` //...and attaches the users token to its header
            }
          })
        }
      }
    })
    return next.handle(request).pipe(); //sends control to next handler in the pipeline
  }
}
