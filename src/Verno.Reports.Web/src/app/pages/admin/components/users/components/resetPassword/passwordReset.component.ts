import { Component, ViewEncapsulation, Output, EventEmitter, ViewChild, OnInit } from '@angular/core';
import { ModalDirective } from 'ng2-bootstrap/ng2-bootstrap';

import { UserDtoImpl } from "../../user.model";
import identity = abp.services.identity;

@Component({
    selector: 'password-reset',
    encapsulation: ViewEncapsulation.None,
    template: require('./passwordReset.html')
})
export class PasswordReset implements OnInit {
    @ViewChild('windowModal') public windowModal: ModalDirective;

    @Output('confirm') confirmEmitter: EventEmitter<any> = new EventEmitter<any>();
    @Output('cancel') cancelEmitter: EventEmitter<any> = new EventEmitter<any>();
    @Output('loaded') loadedEmitter: EventEmitter<PasswordReset> = new EventEmitter<PasswordReset>();

    public title: string;
    private model: identity.passwordResetInput = {};

    ngOnInit() {
        this.loadedEmitter.next(this);
    }

    cancel() {
        this.cancelEmitter.emit(null);
        this.windowModal.hide();
    }

    save(isValid: boolean) {
        abp.ui.setBusy("#passwordResetModal .modal-dialog",
        {
            blockUI: true,
            promise: identity.user.passwordReset(this.model)
                .done(() => {
                    this.confirmEmitter.emit(null);
                    this.windowModal.hide();
                })
        });
    }

    showModal(user: identity.userDto) {
        this.title = "Сброс пароля для "+user.name;
        this.model.userId = user.id;
        this.windowModal.show();
    }
}