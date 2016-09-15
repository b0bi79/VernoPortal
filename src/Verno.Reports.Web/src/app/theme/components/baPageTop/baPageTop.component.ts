import {Component, ViewEncapsulation} from '@angular/core';

import {GlobalState} from '../../../global.state';
import {BaProfilePicturePipe} from '../../pipes';
//import {BaMsgCenter} from '../../components/baMsgCenter';
import {BaScrollPosition} from '../../directives';
import { IdentityService } from '../../../identity';

@Component({
  selector: 'ba-page-top',
  styles: [require('./baPageTop.scss')],
  template: require('./baPageTop.html'),
  directives: [/*BaMsgCenter, */BaScrollPosition],
  pipes: [BaProfilePicturePipe],
  providers: [IdentityService],
  encapsulation: ViewEncapsulation.None
})
export class BaPageTop {

  public isScrolled:boolean = false;
  public isMenuCollapsed: boolean = false;

  constructor(private _state: GlobalState, private _identityService: IdentityService) {
    this._state.subscribe('menu.isCollapsed', (isCollapsed) => {
      this.isMenuCollapsed = isCollapsed;
    });
  }

  public toggleMenu() {
    this.isMenuCollapsed = !this.isMenuCollapsed;
    this._state.notifyDataChanged('menu.isCollapsed', this.isMenuCollapsed);
  }
  public getShownUserName() {
      return this._state.user.name;
  }

  public signout() {
      this._identityService.logout();
  }

  public scrolledChanged(isScrolled) {
    this.isScrolled = isScrolled;
  }
}
