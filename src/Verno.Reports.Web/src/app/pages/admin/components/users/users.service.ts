import { Injectable } from '@angular/core';
import { Http, Headers } from '@angular/http';

import 'rxjs/add/operator/toPromise';

import { User } from './users.model';

function toPromise<T>($promise: abp.IGenericPromise<T>): Promise<T> {
  return new Promise<T>((resolve, reject) => {
    $promise.done(resolve).fail(reject);
  });
}

@Injectable()
export class UsersService {

  private headers = new Headers({ 'Content-Type': 'application/json' });
  private apiUrl = abp.appPath + 'api/services/identity/users';  // URL to web api

  constructor(private http: Http) { }

  getUsers(): Promise<abp.List<User>> {
    return toPromise(abp.services.identity.user.getAll());
    /*return this.http.get(this.apiUrl)
        .toPromise()
        .then(response => response.json().data as User[])
        .catch(this.handleError);*/
  }

  getUser(id: number): Promise<User> {
    return this.getUsers()
      .then(users => users.items.find(user => user.id === id));
  }

  delete(id: number): Promise<User> {
    return toPromise(abp.services.identity.user.delete(id));
    /*let url = `${this.apiUrl}/${id}`;
    return this.http.delete(url, { headers: this.headers })
      .toPromise()
      .then(res => res.json().data)
      .catch(this.handleError);*/
  }

  create(user: User): Promise<User> {
    return toPromise(abp.services.identity.user.create(user));
    /*return this.http
      .post(this.apiUrl, JSON.stringify(user), { headers: this.headers })
      .toPromise()
      .then(res => res.json().data)
      .catch(this.handleError);*/
  }

  update(user: User): Promise<User> {
    return toPromise(abp.services.identity.user.update(user));
    /*const url = `${this.apiUrl}/${user.id}`;
    return this.http
      .put(url, JSON.stringify(user), { headers: this.headers })
      .toPromise()
      .then(() => user)
      .catch(this.handleError);*/
  }

  passwordReset(userId: number, newPassword: string, confirmPassword: string): Promise<void> {
    return toPromise(abp.services.identity.user.passwordReset(userId, { newPassword: newPassword, confirmPassword: confirmPassword}));
    /*let url = `${this.apiUrl}/${userId}/PasswordReset`;
    return this.http
      .post(url, JSON.stringify({ newPassword: newPassword, confirmPassword: confirmPassword }), { headers: this.headers })
      .toPromise()
      .then(() => null)
      .catch(this.handleError);*/
  }

  getRoles(userId: number): Promise<abp.List<string>> {
    return toPromise(abp.services.identity.user.getRoles(userId));
    /*let url = `${this.apiUrl}/${userId}/roles`;
    return this.http.get(url)
      .toPromise()
      .then(response => response.json().data as string[])
      .catch(this.handleError);*/
  }

  updateRoles(userId: number, roleNames: string[]): Promise<void> {
    return toPromise(abp.services.identity.user.updateRoles(userId, roleNames));
    /*let url = `${this.apiUrl}/${userId}/roles`;
    return this.http
      .put(url, JSON.stringify(roleNames), { headers: this.headers })
      .toPromise()
      .then(() => null)
      .catch(this.handleError);*/
  }

  prohibitPermission(userId: number, permissionName: string): Promise<void> {
    return toPromise(abp.services.identity.user.prohibitPermission(userId, permissionName));
    /*let url = `${this.apiUrl}/${userId}/permissions/${permissionName}`;
    return this.http.delete(url, { headers: this.headers })
      .toPromise()
      .then(res => res.json().data)
      .catch(this.handleError);*/
  }

  private handleError(error: any): Promise<any> {
    console.log('Произошла ошибка', error);
    return Promise.reject(error.message || error);
  }
}