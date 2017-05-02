import { Component, ViewEncapsulation, ViewChild } from '@angular/core';
import { Subject } from 'rxjs/Subject';
import { Observable } from 'rxjs/Observable';

import { WindowViewContainerComponent, WindowViewService } from 'ng2-window-view';

import { Return } from '../../returns.model';

@Component({
  selector: 'pack-download',
  encapsulation: ViewEncapsulation.None,
  template: require('./packDownload.html'),
  styles: [`
pack-download .card { background: #007ec3; }
.remove-item { cursor: pointer; }
pack-download li { list-style-type: none; }
`]
})
export class PackDownload {
  @ViewChild(WindowViewContainerComponent)
  windowViewContainer: WindowViewContainerComponent;
  private progress: boolean;

  public items: Return[];

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

  private removeItem(item: Return): void {
    var idx = this.items.indexOf(item);
    if (idx >= 0)
      this.items.splice(idx, 1);
  }
}
