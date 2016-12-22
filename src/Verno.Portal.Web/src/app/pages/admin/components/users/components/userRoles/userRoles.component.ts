import { Component, ViewEncapsulation, ViewChild, OnInit, ElementRef } from '@angular/core';
import { ActivatedRoute, Params } from '@angular/router';
import { Location } from '@angular/common';

import * as users from "../../users.model";
import { UsersService, RolesService } from "../../index";

@Component({
  selector: 'user-roles',
  encapsulation: ViewEncapsulation.None,
  template: require('./userRoles.html'),
  styles: [require('./userRoles.scss')],
  providers: [UsersService, RolesService]
})
export class UserRolesEdit implements OnInit {
  @ViewChild('select') selectElRef;
  public title: string;
  public roles: users.RoleDto[];
  private userRoles: string[];
  private _user: users.User;

  constructor(
    private service: UsersService,
    private rolesService: RolesService,
    private route: ActivatedRoute,
    private location: Location,
    private element: ElementRef
  ) { }

  ngOnInit() {
    var self = this;

    this.route.params.forEach((params: Params) => {
      let id = +params['id'];
      this.setBusy(this.service.getUser(id).then(user => this.user = user));
    });

    this.rolesService.getAll().then(result => {
      self.roles = result.items;
    });
  }

  save() {
    this.setBusy(this.service.updateRoles(this.user.id, this.userRoles).then(() => this.goBack()));
  }

  onSelect(event): void {
    event.target.selected = !event.target.selected;
    if (event.target.selected)
      this.userRoles.push(event.target.value);
    else {
      //this.userRoles = this.userRoles.filter(x => (x !== event.target.value));
      var index = this.userRoles.indexOf(event.target.value, 0);
      if (index > -1) {
        this.userRoles.splice(index, 1);
      }
    }
    event.preventDefault();
  }

  updateSelectList() {
    let options = this.selectElRef.nativeElement.options;
    for (let i = 0; i < options.length; i++) {
      options[i].selected = this.userRoles.indexOf(options[i].value) > -1;
    }
  }

  set user(value: users.User) {
    var self = this;
    this.title = "Роли для " + value.name;
    this._user = value;
    this.setBusy(this.service.getRoles(self._user.id).then(result => {
      self.userRoles = result.items;
      this.updateSelectList();
    }));
  }
  get user(): users.User { return this._user; }

  private goBack(): void {
    this.location.back();
  }

  private setBusy(promise: any): void {
    abp.ui.setBusy(jQuery('.card-body', this.element.nativeElement),
      {
        blockUI: true,
        promise: promise
      });
  }
}