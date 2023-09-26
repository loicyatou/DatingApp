import { CanActivateFn } from '@angular/router';
import { AccountService } from '../_services/account.service';
import { inject } from '@angular/core';
import { map } from 'rxjs/internal/operators/map';
import { ToastrService } from 'ngx-toastr';

//angular dependency ng g guard etc which allows you to block users from navigating onto pages they do not have permission to navigate too 


export const authGuard: CanActivateFn = (route, state) => {
  const accountService = inject(AccountService); //since this isnt a class angular provides an alterative to dependcy inject
  const toastr = inject(ToastrService);

  return accountService.currentUserSource$.pipe( //if there is no user currently stored in our global observable then it will present the user with an error message when a navigation attemp to an unauthorised page is made
    map(user => {
      if (user) {
        return true;
      } else {
        toastr.error('You do not have access to this page!');
        return false;
      }
    })
  )
};
