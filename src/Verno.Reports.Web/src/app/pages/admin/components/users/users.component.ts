import { Component, ViewEncapsulation, OnInit, NgZone } from '@angular/core';
//import { SimpleModal, SimpleModalType } from 'app/theme/components/modal/simple-modal';

import { UserDtoImpl } from './user.model';
import { UserEdit } from "./components/userEdit/userEdit.component"
import { UserRolesEdit } from "./components/userRoles/userRoles.component"
import { PasswordReset } from "./components/resetPassword/passwordReset.component"

import identity = abp.services.identity;

@Component({
    selector: 'users',
    //encapsulation: ViewEncapsulation.None,
    styles: [require('./users.scss')],
    template: require('./users.html'),
    //changeDetection: ChangeDetectionStrategy.OnPush,
})
export class Users implements OnInit {
    filter: string;
    private datas: identity.userDto[];
    private filteredDatas: identity.userDto[];
    private editModal: UserEdit;
    private rolesModal: UserRolesEdit;
    private passwordResetModal: PasswordReset;
    private selectedRow: identity.userDto;

    /*constructor(private modal: SimpleModal) {
    }*/

    ngOnInit() {
        this.getDatas();
    }

    userEditLoaded(modal: UserEdit) { this.editModal = modal; }
    userRolesLoaded(modal: UserRolesEdit) { this.rolesModal = modal; }
    resetPasswordLoaded(modal: PasswordReset) { this.passwordResetModal = modal; }

    getDatas() {
        var self = this;
        identity.user.getAll().done(result => {
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

    showEdit(user?: identity.userDto) {
        this.editModal.showModal(user);
    }

    showRoles(user: identity.userDto) {
        this.rolesModal.showModal(user);
    }

    resetPassword(user: identity.userDto) {
        this.passwordResetModal.showModal(user);
    }

    deleteUser(user: identity.userDto) {
        var self = this;
        abp.message.confirm('Пользователь будет удалён.', 'Вы уверены?',
            isConfirmed => {
                if (isConfirmed) {
                    identity.user.delete(user).done(user => {
                        self.datas = self.datas.filter(x=>x.id !== user.id);
                        self.filteredDatas = self.datas;
                    });
                }
            }
        );
    }

    onRowClick(event, row: identity.userDto) {
        this.selectedRow = row;
        jQuery(event.currentTarget).siblings().removeClass("active");
        jQuery(event.currentTarget).addClass("active");
    }

    userSaved(event) {
        if (event.isNew) {
            this.datas = [...this.datas, event.user];
        } else {
            var idx: number = -1;
            for (var i = 0; i < this.datas.length; i++) {
                if (this.datas[i].id == event.user.id) {
                    idx = i;
                    break;
                }
            }
            if (idx >= 0)
                this.datas[idx] = event.user;
        }
        this.filteredDatas = this.datas;
    }

    public isGranted(name: string): boolean {
        return abp.auth.isGranted(name);
    }
    //TODO password reset
    //TODO если пустой пароль - заставить изменить
}
