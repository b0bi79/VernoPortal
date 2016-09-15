import { Pipe, PipeTransform } from '@angular/core'

// Tell Angular2 we're creating a Pipe with TypeScript decorators
@Pipe({
    name: 'printFilter'
})
export class PrintFilterPipe implements PipeTransform {
    transform(value: abp.services.app.IPrintDocument[], filter: string) {
        return value.filter(doc => !filter
            || doc.imahDok.indexOf(filter) >= 0
            || doc.srcWhId.toString() === filter
            || doc.nomNakl.indexOf(filter) >= 0);
    }
}