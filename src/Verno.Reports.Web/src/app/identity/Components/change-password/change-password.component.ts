import { Component, ViewEncapsulation, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import identity = abp.services.identity;

@Component({
    selector: 'change-password',
  encapsulation: ViewEncapsulation.None,
  styles: [ require('./change-password.scss') ],
  template: require('./change-password.html')
})
export class ChangePassword implements OnInit {
    private user: identity.userLoginInfoDto = { };
    private model: identity.changePasswordInput = { oldPassword: "", newPassword: "", confirmPassword:""};
    constructor(private router: Router) {
    }

    ngOnInit() {
        identity.session.getCurrentLoginInformations()
            .done(result => {
                this.user = result.user;
            });
    }

    save(model: identity.changePasswordInput, isValid: boolean) {
        if (isValid) {
            identity.account.changePassword(model)
                .done(() => {
                    this.router.navigate(['/profile']);
                    abp.message.success("Пароль успешно изменён.", "Пароль сохранён");
                });
        }
    }
}