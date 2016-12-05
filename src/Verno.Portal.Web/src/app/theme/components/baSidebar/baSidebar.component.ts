import { Component, ElementRef, HostListener, ViewEncapsulation } from '@angular/core';
import { Route } from '@angular/router';
import { GlobalState } from '../../../global.state';
import { layoutSizes } from '../../../theme';
import { MENU } from '../../../../app/app.menu';
import * as _ from 'lodash';

@Component({
  selector: 'ba-sidebar',
  encapsulation: ViewEncapsulation.None,
  styles: [require('./baSidebar.scss')],
  template: require('./baSidebar.html')
})
export class BaSidebar {

  // here we declare which routes we want to use as a menu in our sidebar
  //public routes = _.cloneDeep(MENU); // we're creating a deep copy since we are going to change that object
  public routes = BaSidebar.convertToRoutes(_.cloneDeep(abp.nav.menus.MainMenu)); // we're creating a deep copy since we are going to change that object

  public menuHeight:number;
  public isMenuCollapsed:boolean = false;
  public isMenuShouldCollapsed:boolean = false;


  constructor(private _elementRef:ElementRef, private _state:GlobalState) {

    this._state.subscribe('menu.isCollapsed', (isCollapsed) => {
      this.isMenuCollapsed = isCollapsed;
    });
  }

  public ngOnInit():void {
    if (this._shouldMenuCollapse()) {
      this.menuCollapse();
    }
  }

  public ngAfterViewInit():void {
    setTimeout(() => this.updateSidebarHeight());
  }

  @HostListener('window:resize')
  public onWindowResize():void {

    var isMenuShouldCollapsed = this._shouldMenuCollapse();

    if (this.isMenuShouldCollapsed !== isMenuShouldCollapsed) {
      this.menuCollapseStateChange(isMenuShouldCollapsed);
    }
    this.isMenuShouldCollapsed = isMenuShouldCollapsed;
    this.updateSidebarHeight();
  }

  public menuExpand():void {
    this.menuCollapseStateChange(false);
  }

  public menuCollapse():void {
    this.menuCollapseStateChange(true);
  }

  public menuCollapseStateChange(isCollapsed:boolean):void {
    this.isMenuCollapsed = isCollapsed;
    this._state.notifyDataChanged('menu.isCollapsed', this.isMenuCollapsed);
  }

  public updateSidebarHeight():void {
    // TODO: get rid of magic 84 constant
    this.menuHeight = this._elementRef.nativeElement.childNodes[0].clientHeight - 84;
  }

  private _shouldMenuCollapse():boolean {
    return window.innerWidth <= layoutSizes.resWidthCollapseSidebar;
  }

  static convertToRoutes(menu): Route[] {
    var result: Route = {
      path: "pages",
      children: BaSidebar.convertMenuArray(menu.items)
    };
    return [result];
  }

  static convertMenuArray(menuItems): Route[] {
    var result: Route[] = [];
    for (var i = 0; i < menuItems.length; i++) {
      var item = menuItems[i];
      result.push({
        path: item.name,
        data: {
          menu: {
            title: item.displayName,
            icon: item.icon,
            selected: item.customData && item.customData.selected,
            expanded: item.customData && item.customData.expanded,
            order: item.order
          }
        },
        children: BaSidebar.convertMenuArray(item.items)
      });
    }
    return result;
  }
}
