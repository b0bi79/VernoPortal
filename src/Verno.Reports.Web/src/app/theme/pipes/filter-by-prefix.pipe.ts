import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
    name: 'filterByPrefix',
    // http://stackoverflow.com/questions/34456430/ngfor-doesnt-update-data-with-pipe-in-angular2
    pure: false
})
export class FilterByPrefixPipe implements PipeTransform {

    transform(array: Array<string>, prefix: string): Array<string> {
        prefix = prefix.toLowerCase();
        return array.filter(value => value.toLowerCase().startsWith(prefix));
    }
}