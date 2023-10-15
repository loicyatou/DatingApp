import { Directive, Input, OnInit, TemplateRef, ViewContainerRef } from '@angular/core';
import { User } from '../_model/users';
import { AccountService } from '../_services/account.service';
import { take } from 'rxjs';

@Directive({
  selector: '[appHasRole]' 
})
export class HasRoleDirective implements OnInit {
  @Input() appHasRole: string[] = []; //an arary of roles that are allowed to see the element. this is passed in from the tempalte where the directive is
  user: User = {} as User;

  constructor(private viewContainerRef: ViewContainerRef, private templateRef: TemplateRef<any>,
    private accountService: AccountService) {

    this.accountService.currentUserSource$.pipe(take(1)).subscribe({
      next: user => {
        if (user) this.user = user
      } //mits the currently logged in user. The `take(1)` operator ensures that the subscription is automatically unsubscribed after receiving one value.
    })
  }

  ngOnInit(): void {

    if (this.user.roles.some(r => this.appHasRole.includes(r))) { //the directive checks if the user has any roles that match the roles specified in the `appHasRole` input property. It uses the `some` array method to iterate over the user roles and check if any of them are included in the `appHasRole` array.

      this.viewContainerRef.createEmbeddedView(this.templateRef) //f there is a match, the directive creates an embedded view using the `createEmbeddedView` method of the `viewContainerRef` and passing the `templateRef` as an argument. This means that the element associated with the directive will be displayed.
    } else {
      this.viewContainerRef.clear(); //If there is no match, the directive clears the view container using the `clear` method of the `viewContainerRef`. This means that the element associated with the directive will be hidden.
    }
  }

}
