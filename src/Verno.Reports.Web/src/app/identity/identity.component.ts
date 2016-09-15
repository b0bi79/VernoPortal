import {Component, ViewEncapsulation} from '@angular/core';

@Component({
  selector: 'account',
  encapsulation: ViewEncapsulation.None,
  styles: [],
/*     <ba-sidebar></ba-sidebar>   */
  template: `
    <ba-page-top></ba-page-top>
    <div class="al-main">
      <div class="al-content">
        <router-outlet></router-outlet>
      </div>
    </div>
    <footer class="al-footer clearfix">
      <div class="al-footer-main clearfix">
        <div class="al-copy">&copy; <a href="www.verno-info.ru/">Верный</a> 2016</div>
      </div>
    </footer>
    <ba-back-top position="200"></ba-back-top>
    `
})
export class Account {

  constructor() {
  }

  ngOnInit() {
  }
}
