import { Component, Input } from '@angular/core';
import { BrowserXhr } from '@angular/http';

// Use Filesaver.js to save binary to file
// https://github.com/eligrey/FileSaver.js/
let saver = require("file-saver");

@Component({
    selector: 'pdf-downloader',
    //styles: [require('./downloader.scss')],
    template: `
        <button
           class="btn btn-info-outline btn-sm "
          (click)="download()">
            <span class="fa fa-download" *ngIf="!pending"></span>
            <span class="fa fa-refresh fa-spin" *ngIf="pending"></span>
        </button>
        <button
           class="btn btn-info-outline btn-sm "
          (click)="view()">
            <span class="fa fa-eye" *ngIf="!pending"></span>
            <span class="fa fa-refresh fa-spin" *ngIf="pending"></span>
        </button>
        `
})
export class PdfDownloader {

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

    private xhrdownload(callback) {

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
                var blob = new Blob([this.response], { type: 'application/pdf' });
                callback(blob, self.fileName);
            }
        };

        // Start the Ajax request
        xhr.send();
    }
}
