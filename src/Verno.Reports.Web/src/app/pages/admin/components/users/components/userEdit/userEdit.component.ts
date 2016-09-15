import { Component, Output, EventEmitter, ViewChild, OnInit } from '@angular/core';
import { ModalDirective } from 'ng2-bootstrap/ng2-bootstrap';

import { UserDtoImpl } from "../../user.model";

import identity = abp.services.identity;

@Component({
    selector: 'user-edit',
    template: require('./userEdit.html')
})
export class UserEdit implements OnInit {
    @ViewChild('editModal') public editModal: ModalDirective;

    @Output('confirm') confirmEmitter: EventEmitter<any> = new EventEmitter<any>();
    @Output('cancel') cancelEmitter: EventEmitter<any> = new EventEmitter<any>();
    @Output('loaded') loadedEmitter: EventEmitter<UserEdit> = new EventEmitter<UserEdit>();

    public title: string;
    private isNew: boolean;
    public data: identity.userDto = new UserDtoImpl();
    private srcData: identity.userDto;
    private orgUnit: any;

    public items: Array<string> = [];
    public orgUnits: Array<identity.orgUnit> = [];

    ngOnInit() {
        var self = this;

        this.loadedEmitter.next(this);

        identity.orgUnit.getAll().done(result => {
            self.items = result.items.map(x=>x.name);
            self.orgUnits = result.items;
        });
    }

    cancel(): void {
        this.cancelEmitter.emit(null);
        this.editModal.hide();
    }

    save(isValid: boolean): void {
        if (!isValid)
            return;

        if (this.isNew)
            abp.ui.setBusy("#userEditModal .modal-dialog",
                {
                    blockUI: true,
                    promise: identity.user.create(this.data)
                        .done((result: identity.userDto) => {
                            this.confirmEmitter.emit({ user: result, isNew: this.isNew });
                            this.editModal.hide();
                        })
                });
        else
            abp.ui.setBusy("#userEditModal .modal-dialog",
                {
                    blockUI: true,
                    promise: identity.user.update(this.data)
                        .done((result: identity.userDto) => {
                            for (var attr in this.srcData) {
                                if (this.srcData.hasOwnProperty(attr)) this.srcData[attr] = result[attr];
                            }
                            this.confirmEmitter.emit({ user: result, isNew: this.isNew });
                            this.editModal.hide();
                        })
                });
    }

    orgUnitSelected(value: any): void {
        var idx = this.items.indexOf(value);
        this.data.orgUnit = this.orgUnits[idx];
        this.data.orgUnitId = this.data.orgUnit.id;
    }

    showModal(user?: identity.userDto): void {
        var self = this;
        this.title = user ? "Редактирование пользователя" : "Новый пользователь";
        this.srcData = user;
        this.isNew = user == null;
        this.data = user
            ? jQuery.extend(true, {}, user) // clone user for rollback when canceled
            : new UserDtoImpl();            // create new user
        this.orgUnit = self.data.orgUnit.name;
        this.editModal.show();
    }
}