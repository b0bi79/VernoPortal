import { Component, ViewEncapsulation, NgZone } from '@angular/core';

import { GlobalState } from '../../../global.state';

@Component({
  selector: 'user-profile',
  encapsulation: ViewEncapsulation.None,
  template: require('./user-profile.html')
})
export class UserProfile {
  constructor(private state: GlobalState) { }
}