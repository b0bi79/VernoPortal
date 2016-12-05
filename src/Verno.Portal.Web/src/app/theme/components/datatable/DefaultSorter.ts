import { Component, Input } from "@angular/core";
import { DataTable, SortEvent } from "./DataTable";

@Component({
  selector: "mfDefaultSorter",
  styles: [`
span.sort.asc, span.sort.desc {
    content: '';
    display: inline-block;
    width: 0;
    height: 0;
    border-bottom: 4px solid rgba(0, 0, 0, 0.3);
    border-top: 4px solid transparent;
    border-left: 4px solid transparent;
    border-right: 4px solid transparent;
    margin-bottom: 2px;
}
span.sort.desc {
    transform: rotate(-180deg);
    margin-bottom: -2px;
}
`],
  template: `
        <span style="cursor: pointer" (click)="sort()" class="text-nowrap">
            <ng-content></ng-content>
            <span *ngIf="isSortedByMeAsc" class="sort asc" aria-hidden="true"></span>
            <span *ngIf="isSortedByMeDesc" class="sort desc" aria-hidden="true"></span>
        </span>`
})
export class DefaultSorter {
  @Input("by") private sortBy: string;

  private isSortedByMeAsc: boolean = false;
  private isSortedByMeDesc: boolean = false;

  public constructor(private mfTable: DataTable) {
    mfTable.onSortChange.subscribe((event: SortEvent) => {
      this.isSortedByMeAsc = (event.sortBy === this.sortBy && event.sortOrder === "asc");
      this.isSortedByMeDesc = (event.sortBy === this.sortBy && event.sortOrder === "desc");
    })
  }

  private sort() {
    if (this.isSortedByMeAsc) {
      this.mfTable.setSort(this.sortBy, "desc");
    } else {
      this.mfTable.setSort(this.sortBy, "asc");
    }
  }
}