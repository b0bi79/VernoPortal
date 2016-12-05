import {
    Component, ViewEncapsulation, Output, Input, EventEmitter,
    ViewChild, OnInit, Directive, ContentChildren, QueryList, Self,
    ElementRef, ComponentRef
} from '@angular/core';
import { NgForm } from '@angular/forms';
import { ModalDirective } from 'ng2-bootstrap/ng2-bootstrap';

@Directive({ selector: '[form-container]' })
export class HtmlForm {
    constructor(private _element: ElementRef, private _component: ComponentRef<NgForm>) { }

    public submit() {
        var form = jQuery(this._element.nativeElement).find('form');
        form.submit();
    }
}

@Component({
    selector: 'modal-dialog',
    encapsulation: ViewEncapsulation.None,
    template: `
<div bsModal #modalDialog="bs-modal" (onCancel)="cancel()" (onConfirm)="confirm()" id="modalDialog"
     class="modal fade" aria-hidden="true" tabindex="-1" role="dialog">
    <div class="modal-dialog">
        <div class="modal-content">
            <div class="modal-header">
                <button type="button" class="close" data-dismiss="modal" (click)="cancel('Cancel')">
                    <span>&times;</span><span class="sr-only">Close</span>
                </button>
                <img *ngIf="icon" class="modal-icon" style="width:24px;position:relative;top:-2px;" [src]="icon" alt="" title="" />
                <h4 class="modal-title" style="display:inline-block;" id="modal-title" [innerHTML]="title"></h4>
            </div>
            <div class="modal-body" form-container>
                <ng-content></ng-content>
            </div>
            <div class="modal-footer">
                <a class="btn btn-default margin-top-5" (click)="cancel()">Отмена</a>
                <input class="btn btn-primary margin-top-5" [disabled]="form && !form.valid" type="submit" (click)="confirm()" value="{{commitText}}" />
            </div>
        </div>
    </div>
</div>
`
})
export class ModalDialog implements OnInit {
    @ViewChild('modalDialog') public modalDialog: ModalDirective;
    //@ContentChildren(HtmlForm, true) forms: QueryList<HtmlForm>;
    @ViewChild(HtmlForm) content: HtmlForm;

    @Input('commitText') public commitText: string = "Сохранить";

    @Output('confirm') confirmEmitter: EventEmitter<any> = new EventEmitter<any>();
    @Output('cancel') cancelEmitter: EventEmitter<any> = new EventEmitter<any>();
    @Output('loaded') loadedEmitter: EventEmitter<any> = new EventEmitter<any>();

    public title: string;

    ngOnInit() {
        this.loadedEmitter.next(this);
    }

    cancel(): void {
        this.cancelEmitter.emit(null);
        this.modalDialog.hide();
    }

    confirm(): void {
        abp.ui.setBusy("#modalDialog .modal-dialog",
            {
                blockUI: true,
                promise: new Promise((resolve) => {
                    var args = { closeAllow: true };
                    this.confirmEmitter.emit(args);
                    if (args.closeAllow) {
                        if (this.content) this.content.submit();
                        this.modalDialog.hide();
                    }
                    resolve();
                })  
            });
    }

    show(title: string): void {
        this.title = title;
        this.modalDialog.show();
    }
}
