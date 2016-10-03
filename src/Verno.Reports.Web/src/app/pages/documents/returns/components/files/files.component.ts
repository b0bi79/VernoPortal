import { Component, ViewEncapsulation, Input, ElementRef } from '@angular/core';

import { FileUploader } from 'ng2-file-upload';
import { FileType } from 'ng2-file-upload/components/file-upload/file-type.class';

import app = abp.services.app;

/**
 * A Sample of how simple it is to create a new window, with its own injects.
 */
@Component({
  selector: 'returns-files',
  encapsulation: ViewEncapsulation.None,
  styles: [require('./files.scss')],
  template: require('./files.html')
})
export class FilesModal {
  private _rasxod: number;
  public uploader: FileUploader;
  public hasDropZoneOver: boolean = false;
  public files: app.IReturnFileDto[] = [];

  constructor(private element: ElementRef) {
    this.uploader = new FileUploader({
      allowedMimeType: [...FileType.mime_doc, ...FileType.mime_xsl, "application/pdf", "application/image"],
      autoUpload: false,
      //isHTML5: true,
      //authToken: string;
      //disableMultipart: true,
      //url: abp.appPath + 'api/services/app/returns/' + this.rasxod + '/files',
    });
    this.uploader.onCompleteAll = () => {
      this.refresh();
      this.uploader.clearQueue();
    };
  }

  private fileOver(e: any): void {
    this.hasDropZoneOver = e;
  }
  private fileDroped(files) {
    if (this.isGranted('Documents.Returns.UploadFile')) {
      this.uploader.uploadAll();
    }else{
      this.uploader.cancelAll();
      this.uploader.clearQueue();
    }
  }

  private onAddFiles(event) {
    var files = event.target.files;
    this.uploader.addToQueue(files, this.uploader.options, void 0);
    this.uploader.uploadAll();
  }

  private fileDelete(file: app.IReturnFileDto): void {
    var self = this;
    abp.message.confirm('Файл будет удалён.', 'Вы уверены?',
      isConfirmed => {
        if (isConfirmed) {
          app.returns.deleteFile(file.id).done(file => {
            self.files = self.files.filter(x => x.id !== file.id);
          });
        }
      }
    );
  }

  @Input()
  public set rasxod(value: number) {
    this._rasxod = value;
    if (this.isGranted('Documents.Returns.UploadFile'))
      this.uploader.setOptions({ url: abp.appPath + 'api/services/app/returns/' + this._rasxod + '/files' });
    this.refresh();
  };
  public get rasxod(): number {
    return this._rasxod;
  };

  private refresh(): void {
    abp.ui.setBusy(jQuery('.fileupload-buttonbar', this.element.nativeElement),
      {
        blockUI: true,
        promise: app.returns.getFilesList(this._rasxod)
          .done(list => {
            this.files = list.items;
          })
      });
  }

  public isGranted(name: string): boolean {
    return abp.auth.isGranted(name);
  }
}