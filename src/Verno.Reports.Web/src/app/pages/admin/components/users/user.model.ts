export class UserDtoImpl implements abp.services.identity.userDto {
    orgUnitId: number;
    orgUnit: abp.services.identity.orgUnit = { id: 0, name: "", orgUnitType: "", parentUnitId: 0, code: "" };
    userName: string;
    email: string;
    name: string;
    isActive: boolean;
    bossId: string;
    position: string;
    shopNum: number;
    id: number;
}