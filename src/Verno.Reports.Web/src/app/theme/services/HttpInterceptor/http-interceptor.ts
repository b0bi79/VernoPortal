import { Http, Request, RequestOptionsArgs, Response, ResponseOptions, XHRBackend, RequestOptions, ConnectionBackend, Headers } from '@angular/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { LocationStrategy, HashLocationStrategy } from '@angular/common';
import { Observable, Subscribable } from 'rxjs/Observable';
import * as _ from 'lodash'

import { MyApp } from './app/my-app';
import "app/utils"

@Injectable()
export class AbpHttp extends Http {

  constructor(backend: ConnectionBackend, defaultOptions: RequestOptions, private _router: Router) {
    super(backend, defaultOptions);
  }

  request(url: string | Request, options?: RequestOptionsArgs): Observable<Response> {
    return this.intercept(super.request(url, options));
  }

  get(url: string, options?: RequestOptionsArgs): Observable<Response> {
    return this.intercept(super.get(url, options));
  }

  post(url: string, body: string, options?: RequestOptionsArgs): Observable<Response> {
    return this.intercept(super.post(url, body, this.getRequestOptionArgs(options)));
  }

  put(url: string, body: string, options?: RequestOptionsArgs): Observable<Response> {
    return this.intercept(super.put(url, body, this.getRequestOptionArgs(options)));
  }

  delete(url: string, options?: RequestOptionsArgs): Observable<Response> {
    return this.intercept(super.delete(url, options));
  }

  getRequestOptionArgs(options?: RequestOptionsArgs): RequestOptionsArgs {
    if (options == null) {
      options = new RequestOptions();
    }
    if (options.headers == null) {
      options.headers = new Headers();
    }
    options.headers.append('Content-Type', 'application/json');
    return options;
  }

  intercept(observable: Observable<Response>): Observable<Response> {
    return observable
        .do((x: Response) => {
            var data = x.json();
            if (!data || !data.__abp) {
              //Non ABP related return value
              return x;
            }
            return this.handleResponse(x);
          },
          err => {
            if (!err.data || !err.data.__abp) {
              this.handleNonAbpErrorResponse(err);
            } else {
              this.handleResponse(err);
            }
          }
        )
      /*.catch<Response>((err, source) => {
        if (!err.data || !err.data.__abp) {
          return this.handleNonAbpErrorResponse(err, source);
        } else {
          return this.handleResponse(err, source);
        }

        /*if (err.status == 401 && !_.endsWith(err.url, 'api/auth/login')) {
          this._router.navigate(['/login']);
          return Observable.empty();
        } else {
          return Observable.throw(err);
        }#1#
      })*/;
  }

  defaultError: {
    message: 'An error has occurred!',
    details: 'Error detail not sent by server.';
  };
  defaultError401: {
    message: 'You are not authenticated!',
    details: 'You should be authenticated (sign in) in order to perform this operation.';
  };
  defaultError403: {
    message: 'You are not authorized!',
    details: 'You are not allowed to perform this operation.';
  };
  defaultError404: {
    message: 'Resource not found!',
    details: 'The resource requested could not found on the server.';
  };

  logError(error: string): void {
    abp.log.error(error);
  }

  showError(error): Promise<void> {
    if (error.details) {
      return this.toPromise(abp.message.error(error.details, error.message || this.defaultError.message));
    } else {
      return this.toPromise(abp.message.error(error.message || this.defaultError.message, ""));
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

  handleNonAbpErrorResponse(response: any): void {
    if (response.config.abpHandleError !== false) {
      switch (response.status) {
        case 401:
          this.handleUnAuthorizedRequest(
            this.showError(this.defaultError401),
            abp.appPath
          );
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
  }

  handleUnAuthorizedRequest(messagePromise: Promise<void>, targetUrl: string): void {
    if (messagePromise) {
      messagePromise.then(() => {
        this.handleTargetUrl(targetUrl || abp.appPath);
      });
    } else {
      this.handleTargetUrl(targetUrl || abp.appPath);
    }
  }

  handleResponse(response: Response): void {
/*    var originalData = response.json();

    if (originalData.success === true) {
      var r = new Response(new ResponseOptions({
        body: originalData.result,
        status: response.status,
        statusText: response.statusText,
        headers: response.headers,
        type: response.type,
        url: response.url
      }));
      Observable.resolve(response);

      if (originalData.targetUrl) {
        this.handleTargetUrl(originalData.targetUrl);
      }
    } else if (originalData.success === false) {
      var messagePromise = null;

      if (originalData.error) {
        if (response.config.abpHandleError !== false) {
          messagePromise = this.showError(originalData.error);
        }
      } else {
        originalData.error = this.defaultError;
      }

      this.logError(originalData.error);

      response.data = originalData.error;
      Observable.throw(response);

      if (response.status == 401 && response.config.abpHandleError !== false) {
        this.handleUnAuthorizedRequest(messagePromise, originalData.targetUrl);
      }
    } else { //not wrapped result
      defer.resolve(response);
    }*/
  }
}
