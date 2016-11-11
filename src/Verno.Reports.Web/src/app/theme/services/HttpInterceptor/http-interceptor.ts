import { Http, Request, RequestOptionsArgs, Response, RequestOptions, ConnectionBackend, Headers } from '@angular/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';

import 'rxjs/add/operator/toPromise';
import 'rxjs/add/operator/map'
import 'rxjs/add/operator/catch'
import 'rxjs/add/observable/throw';

@Injectable()
export class AbpHttp extends Http {
  public userOptions: {
    abpHandleError: boolean,
    error?: (sender: AbpHttp, args: any) => any,
    success?: (result, data, response: Response) => any,
  };

  constructor(backend: ConnectionBackend, defaultOptions: RequestOptions) {
    super(backend, defaultOptions);
    this.userOptions = {
      abpHandleError: true
    }
  }

  request(url: string | Request, options?: RequestOptionsArgs): Observable<Response> {
    return super.request(url, options)
      .flatMap(response => {
        var data = response.json();
        if (data && data.__abp) {
          return this.handleResponse(data, response);
        } else {
          return Promise.resolve(response);
        }
      })
      .catch((err) => {
          if (err.message) {
            this.showError({ message: "Неизвестная ошибка", details: err.message });
          } else {
            var data = err.json();
            if (!data || !data.__abp) {
              this.handleNonAbpErrorResponse(err);
            } else {
              this.handleResponse(data, err);
            }
          }
          return Observable.throw(err.message || err);
        }
    );
  }

  post(url: string, body: string, options?: RequestOptionsArgs): Observable<Response> {
    return super.post(url, body, this.getRequestOptionArgs(options));
  }

  put(url: string, body: string, options?: RequestOptionsArgs): Observable<Response> {
    return super.put(url, body, this.getRequestOptionArgs(options));
  }

  getRequestOptionArgs(options?: RequestOptionsArgs): RequestOptionsArgs {
    if (options == null) {
      options = new RequestOptions();
    }
    if (options.headers == null) {
      options.headers = new Headers();
    }

    var security = (<any>abp).security;
    if (!options.headers || options.headers[security.antiForgery.tokenHeaderName] === undefined) {
      options.headers.append(security.antiForgery.tokenHeaderName, security.antiForgery.getToken());
    }

    options.headers.append('Content-Type', 'application/json');
    return options;
  }

  defaultError: {
    message: 'Произошла ошибка!',
    details: 'Информация об ошибке сервером не была предоставлена.',
  }
  defaultError401: {
    message: 'Вы не авторизованы!',
    details: 'Вы должны пройти проверку подлинности (войти) для того, чтобы выполнить эту операцию.'
  }
  defaultError403: {
    message: 'Вы не авторизованы!',
    details: 'Вы не можете выполнить эту операцию.'
  }
  defaultError404: {
    message: 'Ресурсы не найдены!',
    details: 'Запрошенный ресурс не найден на сервере.'
  }

  logError(error: string): void {
    abp.log.error(error);
  }

  showError(error): Promise<void> {
    if (error.details) {
      return this.toPromise(abp.message.error(error.details, error.message || this.defaultError.message));
    } else {
      return this.toPromise(abp.message.error("", error.message || this.defaultError.message));
    }
  }

  toPromise<T>($promise): Promise<T> {
    return new Promise<T>((resolve, reject) => {
      $promise.then(resolve, reject);
    });
  }

  handleTargetUrl(targetUrl: string): void {
    if (!targetUrl) {
      location.href = abp.appPath;
    } else {
      location.href = targetUrl;
    }
  }

  handleNonAbpErrorResponse(response: Response): Promise<any> {
    return new Promise((resolve, reject) => {
      if (this.userOptions.abpHandleError !== false) {
        switch (response.status) {
          case 401:
            this.handleUnAuthorizedRequest(this.showError(this.defaultError401), abp.appPath);
            break;
          case 403:
            this.showError(this.defaultError403);
            break;
          case 404:
            this.showError(this.defaultError404);
            break;
          default:
            this.showError(this.defaultError);
            break;
        }
      }
      reject(response);
      this.userOptions.error && this.userOptions.error(this, response);
    });
  }

  handleUnAuthorizedRequest(messagePromise: Promise<void>, targetUrl: string): void {
    if (messagePromise) {
      messagePromise.then(() => {
        this.handleTargetUrl(targetUrl);
      });
    } else {
      this.handleTargetUrl(targetUrl);
    }
  }

  handleResponse(data: any, response: Response): Promise<any> {
    return new Promise((resolve, reject) => {
      if (data) {
        if (data.success === true) {
          resolve(data.result);
          this.userOptions.success && this.userOptions.success(data.result, data, response);

          if (data.targetUrl) {
            this.handleTargetUrl(data.targetUrl);
          }
        } else if (data.success === false) {
          var messagePromise = null;

          if (data.error) {
            if (this.userOptions.abpHandleError !== false) {
              messagePromise = this.showError(data.error);
            }
          } else {
            data.error = this.defaultError;
          }

          this.logError(data.error);

          reject(data.error);
          this.userOptions.error && this.userOptions.error(this, data.error);

          if (response.status === 401 && this.userOptions.abpHandleError !== false) {
            this.handleUnAuthorizedRequest(messagePromise, data.targetUrl);
          }
        } else { //not wrapped result
          resolve(data);
          this.userOptions.success && this.userOptions.success(data, null, response);
        }
      } else { //no data sent to back
        resolve(response);
        this.userOptions.success && this.userOptions.success(null, null, response);
      }
    });
  }
}
