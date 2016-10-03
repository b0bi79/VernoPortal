import { Component, ViewEncapsulation, OnInit, ElementRef } from '@angular/core';
import { ActivatedRoute, Params } from '@angular/router';
import { Location } from '@angular/common';

import { User, UserDtoImpl, OrgUnit, OrgUnitService, UsersService } from "../../index";

@Component({
  selector: 'user-edit',
  encapsulation: ViewEncapsulation.None,
  template: require('./userEdit.html'),
  providers: [UsersService, OrgUnitService]
})
export class UserEdit {
  private isNew: boolean = true;
  private data: User;
  private orgUnit: any;
  private title: string;

  private orgUnitNames: Array<string> = [];
  private orgUnits: Array<OrgUnit> = [];

  constructor(
    private usersService: UsersService,
    private orgUnitService: OrgUnitService,
    private route: ActivatedRoute,
    private location: Location,
    private element: ElementRef
  ) { }

  ngOnInit(): void {
    var self = this;

    this.user = new UserDtoImpl();

    this.route.params.forEach((params: Params) => {
      let id = +params['id'];
      if (id)
        this.setBusy(this.usersService.getUser(id).then(user => this.user = user));
      else
        this.isNew = true;
    });

    this.orgUnitService.getAll().then(result => {
      self.orgUnitNames = result.items.map(x => x.name);
      self.orgUnits = result.items;
    });
  }

  set user(value: User) {
    this.isNew = value == null;
    this.title = this.isNew ? "Новый пользователь" : "Редактирование пользователя";
    this.data = value;
    this.orgUnit = this.data.orgUnit.name;
  }
  get user(): User { return this.data; }

  save(value: User, isValid: boolean) {
    if (!isValid)
      return;

    if (this.isNew)
      this.setBusy(this.usersService.create(this.user).then(() => this.goBack()));
    else
      this.setBusy(this.usersService.update(this.user).then(() => this.goBack()));
  }

  private setBusy(promise: any): void {
    abp.ui.setBusy(jQuery('form', this.element.nativeElement),
      {
        blockUI: true,
        promise: promise
      });    
  }

  private goBack(): void {
    this.location.back();
  }

  private orgUnitSelected(value: any): void {
    var idx = this.orgUnitNames.indexOf(value);
    this.user.orgUnit = this.orgUnits[idx];
    this.user.orgUnitId = this.user.orgUnit.id;
  }
}