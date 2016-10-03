import { Component, ViewEncapsulation, ElementRef } from '@angular/core';
import { ActivatedRoute, Params } from '@angular/router';
import { Location } from '@angular/common';

import * as users from "../../users.model";
import { UsersService } from '../../users.service'

@Component({
  selector: 'password-reset',
  encapsulation: ViewEncapsulation.None,
  template: require('./passwordReset.html'),
  providers: [UsersService]
})
export class PasswordReset {
  private newPassword: string;
  private confirmPassword: string;
  private title: string;
  private user: users.User = new users.UserDtoImpl();

  constructor(
    private service: UsersService,
    private route: ActivatedRoute,
    private location: Location, 
    private element: ElementRef
  ) { }

  ngOnInit(): void {
    this.route.params.forEach((params: Params) => {
      let id = +params['id'];
      this.setBusy(
        this.service.getUser(id)
        .then(user => {
          this.user = user;
          this.title = `Пароль для ${user.name}`;
        })
      );
    });
  }

  save(isValid: boolean) {
    this.setBusy(
      this.service.passwordReset(this.user.id, this.newPassword, this.confirmPassword)
      .then(() => this.goBack())
    );
  }

  private goBack(): void {
    this.location.back();
  }

  private setBusy(promise: any): void {
    abp.ui.setBusy(jQuery('#passwordReset .card', this.element.nativeElement),
      {
        blockUI: true,
        promise: promise
      });
  }
}