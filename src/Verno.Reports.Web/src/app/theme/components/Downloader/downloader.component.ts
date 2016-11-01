import { Component, Input } from '@angular/core';
import { BrowserXhr } from '@angular/http';

// Use Filesaver.js to save binary to file
// https://github.com/eligrey/FileSaver.js/
let saver = require("file-saver");

@Component({
    selector: 'file-downloader',
    //styles: [require('./downloader.scss')],
    template: `
        <button
           class="btn btn-outline-info btn-sm "
          (click)="download()">
            <span class="fa fa-download" *ngIf="!pending"></span>
            <span class="fa fa-refresh fa-spin" *ngIf="pending"></span>
        </button>
        <button
           class="btn btn-outline-info btn-sm "
          (click)="view()">
            <span class="fa fa-eye" *ngIf="!pending"></span>
            <span class="fa fa-refresh fa-spin" *ngIf="pending"></span>
        </button>
        `
})
export class FileDownloader {

    @Input() url: string;
    @Input() fileName: string;

    public pending: boolean = false;

    constructor() { }

    public download() {
        this.xhrdownload((blob: Blob, fileName: string) => saver.saveAs(blob, fileName));
    }

    public view() {
        this.xhrdownload((blob: Blob, fileName: string) => {
            var url = window.URL.createObjectURL(blob);
            window.open(url);
        });
    }

    private xhrdownload(callback: (blob: Blob, fileName: string) => void): void {

        // Xhr creates new context so we need to create reference to this
        let self = this;

        // Status flag used in the template.
        this.pending = true;

        // Create the Xhr request object
        let xhr = new XMLHttpRequest();
        xhr.open('GET', this.url, true);
        xhr.responseType = 'blob';

        // Xhr callback when we get a result back
        // We are not using arrow function because we need the 'this' context
        xhr.onreadystatechange = function () {

            // We use setTimeout to trigger change detection in Zones
            setTimeout(() => { self.pending = false; }, 0);

            // If we get an HTTP status OK (200), save the file using fileSaver
            if (xhr.readyState === 4 && xhr.status === 200) {
                let contentType = xhr.getResponseHeader('Content-Type') || 'application/pdf';
                let fileName = self.getFileName(xhr);
                var blob = new Blob([this.response], { type: contentType });
                callback(blob, self.fileName || fileName);
            }
        };

        // Start the Ajax request
        xhr.send();
    }

    getFileName(xhr: XMLHttpRequest) {
        var filename = "";
        var disposition = xhr.getResponseHeader('Content-Disposition');
        if (disposition && disposition.indexOf('attachment') !== -1) {
            var filenameRegex = /filename[^;=\n]*=((['"]).*?\2|[^;\n]*)/;
            var matches = filenameRegex.exec(disposition);
            if (matches != null && matches[1]) filename = matches[1].replace(/['"]/g, '');
        }
        return filename;
    }
}
