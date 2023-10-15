import { CanActivateFn } from '@angular/router';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';
import { inject } from '@angular/core';
import { map } from 'rxjs';

export const adminGuard: CanActivateFn = (route, state) => {
  const accountSerice = inject(AccountService);
  const toastr = inject(ToastrService);

  //route guard is checking if the user is an admin or moderator before it renders the admin component
  return accountSerice.currentUserSource$.pipe(
    map(user => {
      if(!user) return false;
      if(user.roles.includes('Admin') || user.roles.includes('Moderator')){
        return true;
      } else{
        toastr.error('You cannot enter this area')
        return false;
      }
    })
  )
};
