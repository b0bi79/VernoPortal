import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule as AngularFormsModule } from '@angular/forms';
import { NgaModule } from '../../theme/nga.module';
import { FileUploadModule } from 'ng2-file-upload';
//import { WindowViewModule } from 'ng2-window-view';

import { TooltipModule } from 'ng2-bootstrap/components/tooltip';
import { routing } from './documents.routing';

import { Documents } from './documents.component';
import { Print } from './print';
import { Returns } from './returns';
import { FilesModal } from './returns/components/files';
//import { PackDownload } from './returns/components/packDownload';

@NgModule({
  imports: [
    CommonModule,
    AngularFormsModule,
    NgaModule,
    FileUploadModule,
    TooltipModule,
    //WindowViewModule,
    routing
  ],
  declarations: [
    Print,
    Returns,
    Documents,
    FilesModal,
    //PackDownload
  ],
  // IMPORTANT: 
  // Since PackDownload is never explicitly used (in a template)
  // we must tell angular about it.
  entryComponents: [
    //PackDownload
  ]
})
export default class DocumentsModule { }
