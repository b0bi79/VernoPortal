import { Component, ViewEncapsulation, Input, ElementRef } from '@angular/core';

@Component({
  selector: 'export-table',
  encapsulation: ViewEncapsulation.None,
  /*styles: [require('./exportTable.scss')],*/
  template: require('./exportTable.html')
})
export class ExportTable {
  @Input() table: ElementRef;
  @Input() sheetName: string = "sheet1";
  @Input() fileName: string = "file";
  constructor() {
    
  }

  public ngOnInit(): void {
  }

  public ngAfterViewInit(): void {
  }

  public exportExcel() {

  }
}
