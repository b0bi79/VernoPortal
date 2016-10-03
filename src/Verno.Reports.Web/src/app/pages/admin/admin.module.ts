import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule as AngularFormsModule } from '@angular/forms';
import { NgaModule } from '../../theme/nga.module';

import { routing } from './admin.routing';

import { Admin } from './admin.component';
import { Users } from './components/users';
import { UserEdit, PasswordReset, UserRolesEdit } from "./components/users/components";

@NgModule({
    imports: [
        CommonModule,
        AngularFormsModule,
        NgaModule,
        routing
    ],
    declarations: [
        Users,
        Admin,
        UserEdit,
        PasswordReset,
        UserRolesEdit,
    ],
})
export default class AdminModule { }
