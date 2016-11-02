import { Component, ViewEncapsulation, ViewChild } from '@angular/core';
import { WindowViewContainerComponent, WindowViewService } from 'ng2-window-view';

@Component({
  selector: 'pack-download',
  encapsulation: ViewEncapsulation.None,
  template: require('./packDownload.html')
})
export class PackDownload {
  windowTitle: string = 'Title here!';
  @ViewChild(WindowViewContainerComponent)
  windowViewContainer: WindowViewContainerComponent;

  constructor(private windowView: WindowViewService) {
    
  }

  get position(): { x: number, y: number } { return this.windowViewContainer.position; }
  set position(value: { x: number, y: number }) { this.windowViewContainer.position = value; }

  closeWindow() {
    /**
     * Because order of closing window no longer stable.
     * We have remove window with specific target.
     */
    this.windowView.removeByInstance(this);
  }
}
