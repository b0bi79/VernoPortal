declare module abp.services.app {
  class print {
    static getList(dfrom: string, dto: string, filter: string, httpParams?: any): abp.IGenericPromise<List<IPrintDocument>>;
  }

  interface IPrintDocument {
    id: number;
    liniah: number;
    nomNakl: string;
    dataNakl: Date;
    imahDok: string;
    srcWarehouse: string;
    srcWhId: number;
    url: string;
  }

  class returns {
    static getList(dfrom: string, dto: string, filter: string, unreclaimedOnly: boolean, httpParams?: any): abp.IGenericPromise<List<IReturnDto>>;
    static getFilesList(rasxod: number, httpParams?: any): abp.IGenericPromise<List<IReturnFileDto>>;
    static uploadFile(rasxod: number, file: any, httpParams?: any): abp.IGenericPromise<IReturnFileDto>;
    static deleteFile(fileId: number, httpParams?: any): abp.IGenericPromise<IReturnFileDto>;
  }

  interface IReturnDto {
    id: number;
    shopNum: number;
    docDate: Date;
    docNum: string;
    supplierId: number;
    supplierName: string;
    summ: number;
    liniah: number;
    liniahTip: string;
    returnId: number;
    status: number;
  }

  interface IReturnFileDto {
    id: number;
    returnId: number;
    name: string;
    fileName: string;
    fileSize: number;
    error: string;
    url: string;
  }
}

//declare module vernoIdentity {
declare module abp.services.identity {
  class role {
    static updateRolePermissions(input: updateRolePermissionsInput, httpParams?: any): abp.IPromise;
    static getAll(httpParams?: any): abp.IGenericPromise<List<roleDto>>;
  }
  interface updateRolePermissionsInput {
    roleId: number;
    grantedPermissionNames: string[];
  }

  class account {
    static changePassword(model: changePasswordInput): abp.IPromise;
    static setPassword(model: setPasswordInput): abp.IPromise;
    static hasPassword(): abp.IGenericPromise<boolean>;
  }

  interface changePasswordInput {
    oldPassword: string;
    newPassword: string;
    confirmPassword: string;
  }

  interface setPasswordInput {
    newPassword: string;
    confirmPassword: string;
  }

  class session {
    static getCurrentLoginInformations(httpParams?: any): abp.IGenericPromise<getCurrentLoginInformationsOutput>;
  }
  interface getCurrentLoginInformationsOutput {
    user: userLoginInfoDto;
  }

  interface userLoginInfoDto {
    name?: string;
    userName?: string;
    email?: string;
    id?: number;
    orgUnitId?: number;
    roles: string[];
  }




  class orgUnit {
    static getAll(httpParams?: any): abp.IGenericPromise<List<orgUnit>>;
  }

  class user {
    static prohibitPermission(userId: number, permissionName: string, httpParams?: any): abp.IPromise;
    static updateRoles(userId: number, roleNames: string[], httpParams?: any): abp.IPromise;
    static passwordReset(userId: number, input: passwordResetInput, httpParams?: any): abp.IPromise;
    static getRoles(userId: number, httpParams?: any): abp.IGenericPromise<List<string>>;
    static getAll(httpParams?: any): abp.IGenericPromise<List<userDto>>;
    static create(input: userDto, httpParams?: any): abp.IGenericPromise<userDto>;
    static update(user: userDto, httpParams?: any): abp.IGenericPromise<userDto>;
    static delete(userId: number, httpParams?: any): abp.IGenericPromise<userDto>;
  }

  interface passwordResetInput {
    newPassword?: string;
    confirmPassword?: string;
  }

  interface roleDto {
    id: number;
    name: string;
    application: string;
  }

  interface userDto {
    orgUnitId: number;
    orgUnit: orgUnit;
    userName: string;
    email: string;
    name: string;
    isActive: boolean;
    bossId: string;
    position: string;
    shopNum: number;
    id: number;
  }

  interface orgUnit {
    id: number;
    name: string;
    orgUnitType: string;
    parentUnitId: number;
    code: string;
  }
}

declare module abp {
  var appPath: string;
  class ui {
    static setBusy(element, IPromise);
  }
  class nav {
    static menus: any;
  }
  class auth {
    static isGranted(permissionName: string): boolean;
    static isAnyGranted(args?: string[]): boolean;
    static areAllGranted(args?: string[]): boolean;
  }
  class setting {
    static get(name: string): any;
    static getBoolean(name: string): boolean;
    static getInt(name: string): number;
  }
  class message {
    static info(message: string, title: string);
    static success(message: string, title: string);
    static warn(message: string, title: string);
    static error(message: string, title: string);
    static confirm(message: string, title: string, callback: (isConfirmed: number) => any);
  }
  class notify {
    static info(message: string, title?: string);
    static success(message: string, title?: string);
    static warn(message: string, title?: string);
    static error(message: string, title?: string);
  }
  class localization {
    static languages: any;
    static currentLanguage: any;
  }
  class log {
    static error(message: string): void;
  }
  interface IGenericPromise<T> {
    done(successCallback: (promiseValue: T) => any): IPromise;
    fail(errorCallback: () => any): IPromise;
  }
  interface IPromise {
    done(successCallback: () => any): IPromise;
    fail(errorCallback: () => any): IPromise;
  }
  interface List<T> {
    items: T[];
  }
}
/*declare var abpSessionSvc: abp.services.app.session;
declare var abpUserSvc: abp.services.app.user;
declare var abpRoleSvc: abp.services.app.role;*/
