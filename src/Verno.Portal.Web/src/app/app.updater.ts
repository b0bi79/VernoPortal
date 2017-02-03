import { Injectable } from '@angular/core';
import { Http, Headers } from '@angular/http';
import { Observable } from 'rxjs';

@Injectable()
export class Updater {
  private currentVersion: string = '1.1';

  constructor(private http: Http) {
    let o = Observable.timer(0, 20 * 60 * 1000);
    o.subscribe(() => {
      let headers = new Headers({
        'Cache-Control': 'no-cache, no-store, must-revalidate',
        'Pragma': 'no-cache',
        'Expires': '0'
      });
      this.http.get(abp.appPath + 'version.txt', { headers: headers })
        .toPromise()
        .then(response => {
          var ver = response.text();
          if (ver !== this.currentVersion)
            abp.notify.warn('Вышла новая версия VernoPortal. Нажмите это сообщение для обновления страницы.',
              "Обновление VernoPortal",
              {
                "preventDuplicates": true,
                "timeOut": "-1",
                onclick() { window.location.reload(true); }
              });
        });
    });
  }
}
