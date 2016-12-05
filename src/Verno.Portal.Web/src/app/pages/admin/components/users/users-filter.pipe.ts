/// <reference path="../../../../../app.services-typings.d.ts" />
import { Pipe, PipeTransform } from '@angular/core';

// Tell Angular2 we're creating a Pipe with TypeScript decorators
@Pipe({
    name: 'userFilter',
    pure: false,
})
export class UserFilterPipe implements PipeTransform {
    transform(value: users.User[], filter: string) {
        return value.filter(doc => !filter
            || doc.name.indexOf(filter) >= 0
            || doc.email.indexOf(filter) >= 0
            || doc.userName.indexOf(filter) >= 0
            || (doc.position && doc.position.indexOf(filter) >= 0)
            || (doc.orgUnit && doc.orgUnit.name.indexOf(filter) >= 0)
        );
    }
}