import { Component, ViewEncapsulation, Output, EventEmitter, ViewChild, OnInit } from '@angular/core';
import { NgClass } from '@angular/common';
import { ModalDirective } from 'ng2-bootstrap/ng2-bootstrap';

import { UserDtoImpl } from "../../user.model";
import identity = abp.services.identity;

@Component({
    selector: 'user-roles',
    encapsulation: ViewEncapsulation.None,
    template: require('./userRoles.html'),
    styles: [require('./userRoles.scss')]
})
export class UserRolesEdit implements OnInit {
    @ViewChild('rolesModal') public rolesModal: ModalDirective;

    @Output('confirm') confirmEmitter: EventEmitter<any> = new EventEmitter<any>();
    @Output('cancel') cancelEmitter: EventEmitter<any> = new EventEmitter<any>();
    @Output('loaded') loadedEmitter: EventEmitter<UserRolesEdit> = new EventEmitter<UserRolesEdit>();

    public title: string;
    public roles: identity.roleDto[];
    private userRoles: string[];
    private user: identity.userDto;

    ngOnInit() {
        var self = this;

        this.loadedEmitter.next(this);

        identity.role.getAll().done(result => {
            self.roles = result.items;
        });
    }

    cancel() {
        this.cancelEmitter.emit(null);
        this.rolesModal.hide();
    }

    confirm() {
        abp.ui.setBusy("#userRolesModal .modal-dialog",
        {
            blockUI: true,
            promise: identity.user.updateRoles(this.user.id, this.userRoles)
                .done(() => {
                    this.confirmEmitter.emit(null);
                    this.rolesModal.hide();
                })
        });
    }

    onSelect(event): void {
        event.target.selected = !event.target.selected;
        if (event.target.selected)
            this.userRoles.push(event.target.text);
        else {
            //this.userRoles = this.userRoles.filter(x => (x !== event.target.value));
            var index = this.userRoles.indexOf(event.target.text, 0);
            if (index > -1) {
                this.userRoles.splice(index, 1);
            }
        }
        event.preventDefault();
    }

    onChange(newObj) {
        this.userRoles = newObj;
        console.log(newObj);
    }

    showModal(user: identity.userDto) {
        var self = this;
        this.title = "Роли для "+user.name;
        this.user = user;
        abp.ui.setBusy("#userRolesModal .modal-dialog",
        {
            blockUI: true,
            promise: identity.user.getRoles(self.user.id)
                .done(result => self.userRoles = result.items)
        });
        this.rolesModal.show();
    }
}