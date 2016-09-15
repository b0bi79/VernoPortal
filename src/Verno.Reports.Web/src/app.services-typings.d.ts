declare module abp.services.app {
    class print {
        static getList(dfrom: string, dto: string, httpParams?: any): abp.IGenericPromise<ListPrintDto>;
    }

    interface ListPrintDto {
        items: IPrintDocument[];
    }

    export interface IPrintDocument {
        liniah: number;
        nomNakl: string;
        dataNakl: Date;
        imahDok: string;
        srcWarehouse: string;
        srcWhId: number;
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
    }




    class orgUnit {
        static getAll(httpParams?: any): abp.IGenericPromise<List<orgUnit>>;
    }

    class user {
        static prohibitPermission(input: prohibitPermissionInput, httpParams?: any): abp.IPromise;
        static updateRoles(userId: number, roleNames: string[], httpParams?: any): abp.IPromise;
        static passwordReset(input: passwordResetInput, httpParams?: any): abp.IPromise;
        static getRoles(userId: number, httpParams?: any): abp.IGenericPromise<List<string>>;
        static getAll(httpParams?: any): abp.IGenericPromise<List<userDto>>;
        static create(input: userDto, httpParams?: any): abp.IGenericPromise<userDto>;
        static update(user: userDto, httpParams?: any): abp.IGenericPromise<userDto>;
        static delete(input: userDto, httpParams?: any): abp.IGenericPromise<userDto>;
    }
    interface List<T>{
        items: T[];
    }

    interface prohibitPermissionInput {
        userId: number;
        permissionName: string;
    }

    interface passwordResetInput {
        userId?: number;
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
    interface IGenericPromise<T> {
        done(successCallback: (promiseValue: T) => any): any;
        error(errorCallback: () => any): any;
    }
    interface IPromise {
        done(successCallback: () => any): any;
        error(errorCallback: () => any): any;
    }
}
/*declare var abpSessionSvc: abp.services.app.session;
declare var abpUserSvc: abp.services.app.user;
declare var abpRoleSvc: abp.services.app.role;*/
