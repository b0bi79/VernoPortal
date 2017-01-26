import { Injectable } from '@angular/core';
import { Http } from '@angular/http';
import { Observable } from 'rxjs';

@Injectable()
export class Updater {
  private currentVersion: string = '1.0';

  constructor(private http: Http) {
    let o = Observable.timer(0, 0.5 * 60 * 1000);
    o.subscribe(() => {
      this.http.get(abp.appPath + 'version.txt')
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
