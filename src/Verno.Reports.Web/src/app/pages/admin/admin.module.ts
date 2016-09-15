import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule as AngularFormsModule } from '@angular/forms';
import { NgaModule } from '../../theme/nga.module';

import { routing } from './admin.routing';

import { RatingModule } from 'ng2-bootstrap/ng2-bootstrap';
import { Admin } from './admin.component';
import { Users } from './components/users';


@NgModule({
    imports: [
        CommonModule,
        AngularFormsModule,
        NgaModule,
        RatingModule,
        routing
    ],
    declarations: [
        Users,
        Admin
    ]
})
export default class AdminModule { }
