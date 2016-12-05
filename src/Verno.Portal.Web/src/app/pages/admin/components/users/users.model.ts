export interface RoleDto extends abp.services.identity.roleDto {
}

export interface OrgUnit extends abp.services.identity.orgUnit {
}

export interface User extends abp.services.identity.userDto {
}

export interface UpdateRolePermissionsInput extends abp.services.identity.updateRolePermissionsInput {
}

export class UserDtoImpl implements User {
    orgUnitId: number;
    orgUnit: OrgUnit = { id: 0, name: "", orgUnitType: "", parentUnitId: 0, code: "" };
    userName: string;
    email: string;
    name: string;
    isActive: boolean;
    bossId: string;
    position: string;
    shopNum: number;
    id: number;
}