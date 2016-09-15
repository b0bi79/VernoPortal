import { Component, ViewEncapsulation, Input, OnInit, NgZone } from '@angular/core';
import { Location } from '@angular/common';

@Component({
    selector: 'user-profile',
  encapsulation: ViewEncapsulation.None,
  template: require('./user-profile.html')
})
export class UserProfile implements OnInit {
    private user: abp.services.identity.userLoginInfoDto = { name: "", userName: "", email: "", id: 0, orgUnitId: 0};
    constructor(private location: Location) {
    }

    ngOnInit() {
        abp.services.identity.session.getCurrentLoginInformations()
            .done(result => {
                this.user = result.user;
            });
    }
}