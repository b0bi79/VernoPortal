import { Directive, Input, HostBinding, HostListener } from "@angular/core";
import { DataTable } from "./DataTable";

@Directive({ selector: "[mfSelectable]" })
export class Selectable {
    @Input("mfSelectable") public row: any;

    public constructor(private mfTable: DataTable) {
    }

    @HostListener('click', ['$event']) private onRowClick($event): void {
        this.mfTable.activeRow = this.row;
    }

    @HostBinding('class.active') get active() { return this.mfTable.activeRow === this.row };
}