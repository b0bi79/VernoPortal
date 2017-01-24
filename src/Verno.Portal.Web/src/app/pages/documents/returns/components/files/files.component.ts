import { Component, ViewEncapsulation, Input, Output, EventEmitter, ElementRef } from '@angular/core';

import { FileUploader } from 'ng2-file-upload';
import { FileType } from 'ng2-file-upload/components/file-upload/file-type.class';
import { Return, ReturnFileDto } from '../../returns.model'
import { ReturnsService } from '../../returns.service';

/**
 * A Sample of how simple it is to create a new window, with its own injects.
 */
@Component({
  selector: 'returns-files',
  encapsulation: ViewEncapsulation.None,
  styles: [require('./files.scss')],
  providers: [ReturnsService],
  template: require('./files.html')
})
export class FilesModal {
  private _return: Return;
  public uploader: FileUploader;
  public hasDropZoneOver: boolean = false;
  public files: ReturnFileDto[] = [];

  constructor(private element: ElementRef, private returnsSvc: ReturnsService) {
    this.uploader = new FileUploader({
      allowedMimeType: [...FileType.mime_doc, ...FileType.mime_xsl, ...FileType.mime_compress, "application/zip", "application/pdf", "image/jpeg","image/pjpeg","image/png","image/bmp"],
      autoUpload: false,
      //isHTML5: true,
      //authToken: string;
      //disableMultipart: true,
      //url: abp.appPath + 'api/services/app/returns/' + this.rasxod + '/files',
    });
    this.uploader.onBeforeUploadItem = () => {
      window.onbeforeunload = e => {
        e = e || window.event;
        var message = 'Идёт загрузка файлов!!! Вы действительно хотите уйти со страницы и прервать загрузку?';
        if (e) { // For IE6-8 and Firefox prior to version 4
          e.returnValue = message;
        }
        return message;
      };
    };
    this.uploader.onErrorItem = (item, response, status, headers) => {
      abp.message.error("Попробуйте загрузить файл " + item.file.name + " ещё раз, если ничего не помогло обратитесь в техподдержку.",
        "Ошибка при загрузке файла");
    };
    this.uploader.onCancelItem = (item, response, status, headers) => {
      abp.message.warn("Загрузка файла " + item.file.name+" была прервана или отменена пользователем.",
        "Файл не был загружен");
    };
    this.uploader.onCompleteAll = () => {
      this.refresh();
      this.uploader.clearQueue();
      setTimeout(() => { window.onbeforeunload = null;}, 1000);
    }
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

  private fileDelete(file: ReturnFileDto): void {
    var self = this;
    abp.message.confirm('Файл будет удалён.', 'Вы уверены?',
      isConfirmed => {
        if (isConfirmed) {
          this.returnsSvc.deleteFile(file.id).then(file => {
            self.files = self.files.filter(x => x.id !== file.id);
            this._return.files = this.files;
          });
        }
      }
    );
  }

  @Input()
  public set rasxod(value: Return) {
    this._return = value;
    if (this.isGranted('Documents.Returns.UploadFile'))
      this.uploader.setOptions({ url: abp.appPath + 'api/services/app/returns/' + this._return.id + '/files' });
    if (this._return.files)
      this.files = this._return.files;
    else
      this.refresh();
  };
  public get rasxod(): Return {
    return this._return;
  };

  private refresh(): void {
    abp.ui.setBusy(jQuery('.fileupload-buttonbar', this.element.nativeElement),
      {
        blockUI: true,
        promise: this.returnsSvc.getFilesList(this._return.id)
          .then(list => {
            this.files = list.items;
            this._return.files = this.files;
          })
      });
  }

  public isGranted(name: string): boolean {
    return abp.auth.isGranted(name);
  }
}