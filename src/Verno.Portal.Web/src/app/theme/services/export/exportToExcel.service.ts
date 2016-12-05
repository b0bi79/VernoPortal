import { Injectable } from '@angular/core';
let saver = require("file-saver");
import * as XLSX from 'xlsx';
import { Workbook, Worksheet } from './workbook.model';

@Injectable()
export class ExportToExcelService {
  public export(data: Workbook, fileName: string): void {
    let wb: XLSX.IWorkBook = { Props: null, Sheets: {}, SheetNames: [] };

    for (var i = 0; i < data.sheets.length; i++) {
      this.addSheet(wb, data.sheets[i]);
    }

    let wopts = { bookType: 'xlsx', bookSST: false, type: 'binary' };
    let wbout = XLSX.write(wb, wopts);
    let blob = new Blob([this.str2arrbuf(wbout)], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
    saver.saveAs(blob, fileName);
  }

  private addSheet(wb: XLSX.IWorkBook, sheet: Worksheet): void {
    wb.SheetNames.push(sheet.name);

    let data = sheet.data;
    let ws: XLSX.IWorkSheet = {};
    let range = { s: { c: 10000000, r: 10000000 }, e: { c: 0, r: 0 } };

    this.createHeader(ws, sheet);
    
    for (var r = 0; r !== data.length; ++r) {
      for (var c = 0; c !== sheet.columns.length; ++c) {
        let row = r + 2;
        let val = sheet.columns[c].eval(data[r]);
        range.s.r = 0/*Math.min(range.s.r, row)*/; range.s.c = Math.min(range.s.c, c);
        range.e.r = Math.max(range.e.r, row); range.e.c = Math.max(range.e.c, c);
        let cell: XLSX.IWorkSheetCell = <XLSX.IWorkSheetCell>({ v: val });
        if (val == null) continue;
        let cellRef = XLSX.utils.encode_cell({ c: c, r: row });

        if (typeof val === 'number') cell.t = 'n';
        else if (typeof val === 'boolean') cell.t = 'b';
        else if (val instanceof Date) {
          cell.t = 'n';
          cell.z = (<any>XLSX).SSF._table[14];
          cell.v = <any>this.datenum(val);
        } else cell.t = 's';
        ws[cellRef] = cell;
      }
    }

    if (range.s.c < 10000000) ws['!ref'] = XLSX.utils.encode_range(range.s, range.e);

    wb.Sheets[sheet.name] = ws;
  }

  private getCellWidth(val: any): number {
    if (typeof val === 'number') return 7;
    else if (typeof val === 'boolean') return 6;
    else if (val instanceof Date) return 12;
    else return 40;
  }
  private datenum(v: any, date1904?: boolean): number {
    if (date1904) v += 1462;
    var epoch = Date.parse(v);
    var offset = v.getTimezoneOffset() * 60 * 1000;
    return (epoch - offset - Date.UTC(1899, 11, 30)) / (24 * 60 * 60 * 1000);
  }

  private str2arrbuf(s: string): ArrayBuffer {
    var buf = new ArrayBuffer(s.length);
    var view = new Uint8Array(buf);
    for (var i = 0; i !== s.length; ++i) view[i] = s.charCodeAt(i) & 0xFF;
    return buf;
  }

  private createHeader(ws: XLSX.IWorkSheet, sheet: Worksheet): void {
    var wscols = [];
    if (sheet.data.length > 0) {
      for (var c = 0; c !== sheet.columns.length; ++c) {
        let col = sheet.columns[c];
        let cellWidth = col.width
          ? col.width
          : this.getCellWidth(col.eval(sheet.data[0]));
        wscols.push({ wch: cellWidth });

        let cell: XLSX.IWorkSheetCell = <XLSX.IWorkSheetCell>({ v: col.header, t: 's' });
        let cellRef = XLSX.utils.encode_cell({ c: c, r: 1 });
        ws[cellRef] = cell;
      }
    }
    ws['!cols'] = ((wscols) as any);
  }
}
