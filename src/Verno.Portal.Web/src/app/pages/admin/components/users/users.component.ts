import { Component, ViewChild, ViewEncapsulation, OnInit, NgZone } from '@angular/core';
import { Router } from '@angular/router';

import { Modal, BSModalContext } from 'angular2-modal/plugins/bootstrap';

import { TableOptions, TableColumn, ColumnMode } from 'angular2-data-table';

import * as users from './users.model';
import { UserEdit } from './components/userEdit/userEdit.component'
import { UserRolesEdit } from './components/userRoles/userRoles.component'
import { PasswordReset } from './components/resetPassword/passwordReset.component'
import { UsersService } from './users.service'

@Component({
    selector: 'users',
    //encapsulation: ViewEncapsulation.None,
    styles: [require('./users.scss')],
    template: require('./users.html'),
    providers: [UsersService]
})
export class Users implements OnInit {
    filter: string;
    private datas: users.User[];
    private filteredDatas: users.User[] = [];

    constructor(private service: UsersService, private router: Router) { }

    ngOnInit() {
        this.getDatas();
    }

    getDatas() {
        var self = this;
        this.service.getUsers().then(result => {
            self.datas = result.items;
            self.filteredDatas = result.items;
        });
    }

    public filterData(query: string) {
        console.log(query);
        if (query) {
            query = query.toLowerCase();
            this.filteredDatas = _.filter(this.datas,
                doc => doc.name.toLowerCase().indexOf(query) >= 0
                    || doc.email.toLowerCase().indexOf(query) >= 0
                    || doc.userName.toLowerCase().indexOf(query) >= 0
                    || (doc.position && doc.position.toLowerCase().indexOf(query) >= 0)
                    || (doc.orgUnit && doc.orgUnit.name.toLowerCase().indexOf(query) >= 0));
        } else {
            this.filteredDatas = this.datas;
        }
    }

    editUser(user: users.User) {
      this.router.navigate(["/pages", "admin", "users", user ? user.id : 'new']);
    }

    showRoles(user: users.User) {
      this.router.navigate(["/pages", "admin", "users", user.id, 'roles']);
    }

    resetPassword(user: users.User) {
      this.router.navigate(["/pages", "admin", "users", user.id, 'reset-password']);
    }

    deleteUser(user: users.User) {
        var self = this;
        abp.message.confirm('Пользователь будет удалён.', 'Вы уверены?',
            isConfirmed => {
                if (isConfirmed) {
                  this.service.delete(user.id).then(user => {
                        self.datas = self.datas.filter(x=>x.id !== user.id);
                        self.filteredDatas = self.datas;
                    });
                }
            }
        );
    }
  
    public isGranted(name: string): boolean {
        return abp.auth.isGranted(name);
    }
}
