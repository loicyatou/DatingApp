import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
  HttpErrorResponse
} from '@angular/common/http';
import { Observable, catchError } from 'rxjs';
import { ToastrService } from 'ngx-toastr';
import { NavigationExtras, Router } from '@angular/router';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {

  constructor(private router: Router, private toastr: ToastrService) { }

  //An interceptor: Angular interceptors are a medium class that conncts front and backend. When request is made the interceptros handle it inbetween. they can also be used to identify the response by performing RCJS operators which we are doing below

  //The diff here with middleware is that middleware works within a pipeline on the backend, intercptrors are global and work both front and back. 

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    return next.handle(request).pipe( //passes request to pipe so that it can handle any errors that might occur during the execution of the request it it has caught
      catchError((error: HttpErrorResponse) => {
        if (error) {
          switch (error.status) {
            case 400: 
              if (error.error.errors) { //checks if the error contains a specific structure. If it does it will extract those errors and pass them to an array so that the developer can easily read the array of errors that have arisen
                const err = error.error.errors;
                const modelStateErrors = [];
                for (const key in err) {
                  if (err[key]) {
                    modelStateErrors.push(err[key])
                  }
                }
                throw modelStateErrors.flat(); //throws the error to the console if such an error structure exists
              } else {
                this.toastr.error(error.error, error.status.toString()) //ottherwise it sends a UI notification of the error
              }
              break;
            case 401:
              this.toastr.error('Unauthorised', error.status.toString()) //display sunathorised UI error
              break;
            case 404:
              this.router.navigateByUrl('/not-found') //navigates to a paticular page
              break;
            case 500:
              const navigationExtras: NavigationExtras = { state: { error: error.error } }; //Here we are opting to render the error directly within a specific component or route. This is used for errors that are usually more urgent and require immediate attention by the user unlike a toastr notification. 

              //state: refers to the current data or configuration of an app or specifi componetn in a given moment. here the sate is assaingend the erorr.error which is the actual 500 error and have it displayed to the user

              this.router.navigateByUrl('/server-error', navigationExtras);
              break;
            default:
              this.toastr.error('Something unexpected went wrong');
              console.log(error);
              break;
          }
        }
        throw error;
      })
    );
  }
}
