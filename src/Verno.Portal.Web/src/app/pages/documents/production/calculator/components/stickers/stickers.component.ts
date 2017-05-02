import { Component, ViewEncapsulation, ViewChild } from '@angular/core';
import { Subject } from 'rxjs/Subject';
import { Observable } from 'rxjs/Observable';

import { WindowViewContainerComponent, WindowViewService } from 'ng2-window-view';

import { ProductionDto } from '../../production.model';

@Component({
  selector: 'stickers',
  encapsulation: ViewEncapsulation.None,
  template: require('./stickers.html'),
  styles: [`
stickers .card { background: #007ec3; }
`]
})
export class Stickers {
  @ViewChild(WindowViewContainerComponent)
  windowViewContainer: WindowViewContainerComponent;
  private progress: boolean;
  public items: ProductionDto[];

  private _result$: Subject<boolean> = new Subject<boolean>();
  get result$(): Observable<boolean> { return this._result$.asObservable(); }

  constructor(private windowView: WindowViewService) {
  }

  get position(): { x: number, y: number } { return this.windowViewContainer.position; }
  set position(value: { x: number, y: number }) { this.windowViewContainer.position = value; }

  confirm() {
    this._result$.next(true);
  }

  deny() {
    this._result$.next(false);
  }

  close() {
    this.windowView.removeByInstance(this);
    this._result$.complete();
  }

  showProgress() {
    this.progress = true;
  }
}
