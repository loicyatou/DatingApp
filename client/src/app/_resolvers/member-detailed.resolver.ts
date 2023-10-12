import { ResolveFn } from '@angular/router';
import { Member } from '../_model/members';
import { MembersService } from '../_services/members.service';
import { inject } from '@angular/core';

//What is the purpose of a route revolver: It delays the rendering of a component untill the neccesary data is retrieved. This is useful in scenarios where a component depends on certain data to be fetched from an API or resolved from a service before it can be displayed correctly.

//here we are going to retrieve the member before the compnetn details needs to be displayed so that messages will load when certain buttons are clicked as opposed the if conitin in hte html assuming it isnt there and thus not loading. 

export const memberDetailedResolver: ResolveFn<Member> = (route, state) => { //route: contains info about the current route including route params //state: info about current router state
  const memberService = inject(MembersService); //inject is just a angular function to add dependency injection. it will retreive an instance of the MembersService class

  return memberService.getMember(route.paramMap.get('username')!)// paramMap is a an obejct thaat contains route parameters extracted from the URL The `paramMap` object provides a key-value mapping of the route parameters, where the key is the parameter name and the value is the corresponding value extracted from the URL. In this case, the `username` parameter is accessed using the key `'username'`. So, when the resolver function is executed for a specific route, it retrieves the `username` parameter value from the URL and passes it to the `getMember` method of the `MembersService` to fetch the corresponding member's data.

  //to fully get this go to app-routing module and you will see where its getting the username from
};
