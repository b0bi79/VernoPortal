import { Nomenklatura } from 'app/theme/services/shInfoSql';

export interface ProizvMagSpecDto {
  id: number;
  proekt: number;
  proektName: string;
  date: Date;
  dateFrom: Date;
  dateTo: Date;
  promo: boolean;
  period: { start: Date; end: Date};
  items: ProizvMagSpecItemDto[];
}

export interface ProizvMagSpecItemDto {
  id: number;
  tovar: Nomenklatura;
  norm?: number;
  koeff?: number;
}

export class ProizvMagSpec implements ProizvMagSpecDto {
  id: number = 0;
  proekt: number;
  proektName: string;
  date: Date = new Date();
  dateFrom: Date;
  dateTo: Date;
  promo: boolean = true;
  get period(): { start: Date; end: Date } {
    return { start: this.dateFrom, end: this.dateTo };
  }
  set period(value: { start: Date; end: Date }) {
    this.dateFrom = value.start;
    this.dateTo = value.end;
  }
  items: ProizvMagSpecItemDto[];
}       

export class ProizvMagSpecItem implements ProizvMagSpecItemDto {
  id: number = 0;
  tovar: Nomenklatura;
  norm?: number;
  koeff?: number;
}