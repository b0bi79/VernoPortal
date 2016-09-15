import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule as AngularFormsModule } from '@angular/forms';
import { NgaModule } from '../theme/nga.module';

import { routing } from './identity.routing';

import { RatingModule } from 'ng2-bootstrap/ng2-bootstrap';
import { Account } from './identity.component'
import { UserProfile, ChangePassword } from './Components'


@NgModule({
    imports: [
        CommonModule,
        AngularFormsModule,
        NgaModule,
        RatingModule,
        routing
    ],
    declarations: [
        Account,
        UserProfile,
        ChangePassword
    ]
})
export default class IdentityModule { }
