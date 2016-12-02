import {Component, ViewEncapsulation} from '@angular/core';

@Component({
  selector: 'reports',
  encapsulation: ViewEncapsulation.None,
  styles: [require('./reports.scss')],
  template: require('./reports.html')
})
export class Reports {

  constructor() {
  }

}
