import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule as AngularFormsModule } from '@angular/forms';
import { NgaModule } from '../../theme/nga.module';

import { routing } from './documents.routing';

import { RatingModule } from 'ng2-bootstrap/ng2-bootstrap';
import { Documents } from './documents.component';
import { Print } from './print';
import { Returns } from './returns';


@NgModule({
    imports: [
        CommonModule,
        AngularFormsModule,
        NgaModule,
        RatingModule,
        routing
    ],
    declarations: [
        Print,
        Returns,
        Documents
    ]
})
export default class DocumentsModule { }
