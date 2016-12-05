import { Component, ViewEncapsulation } from '@angular/core';
import { Router } from '@angular/router';
import { IdentityService } from '../../identity.service'

import { GlobalState } from '../../../global.state';
import * as models from '../../identity.model';

//import identity = abp.services.identity;

@Component({
    selector: 'change-password',
  encapsulation: ViewEncapsulation.None,
  styles: [ require('./change-password.scss') ],
  template: require('./change-password.html')
})
export class ChangePassword {
  private model: models.ChangePasswordInput = { oldPassword: "", newPassword: "", confirmPassword:""};
      
    constructor(
      private router: Router,
      private identity: IdentityService,
      private state: GlobalState) {
    }

    save(model: models.ChangePasswordInput, isValid: boolean) {
        if (isValid) {
            this.identity.changePassword(model)
                .then(() => {
                    this.router.navigate(['/profile']);
                    abp.message.success("Пароль успешно изменён.", "Пароль сохранён");
                });
        }
    }
}