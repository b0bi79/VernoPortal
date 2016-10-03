import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule as AngularFormsModule } from '@angular/forms';
import { NgaModule } from '../theme/nga.module';

import { routing } from './identity.routing';

import { Account } from './identity.component'
import { UserProfile, ChangePassword } from './Components'
import { IdentityService } from "./identity.service"

@NgModule({
    imports: [
        CommonModule,
        AngularFormsModule,
        NgaModule,
        routing
    ],
    declarations: [
        Account,
        UserProfile,
        ChangePassword
    ],
    providers: [
      IdentityService
    ]
})
export class IdentityModule { }
