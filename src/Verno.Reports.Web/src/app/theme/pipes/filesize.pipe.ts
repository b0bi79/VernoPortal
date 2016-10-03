import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'filesize' })
export class FileSizePipe implements PipeTransform {

  transform(bytes: number): string {
    if (bytes >= 1000000000) {
      return (bytes / 1024/1024/1024).toFixed(2) + " ГБ";
    }
    if (bytes >= 1000000) {
      return (bytes / 1024 / 1024).toFixed(2) + " МБ";
    }
    return (bytes / 1024).toFixed(2) + " КБ";
  }
}