export interface ProductionDto {
  imahKod2     : string;
  vidTovara    : number;
  shtrixKod    : number;
  naimenovanie : string;
  imahPr       : string;
  realizSht    : number;
  spisSht      : number;
  normativ     : number;
  koeff        : number;
  toBake       : number;
  ostSht: number;
  etiketka: number;
  toPrintSticker: number;
}                                            
export class Production implements ProductionDto {
  imahKod2: string;
  vidTovara: number;
  shtrixKod: number;
  naimenovanie: string;
  imahPr: string;
  realizSht: number;
  spisSht: number;
  normativ: number;
  koeff: number;
  toBake: number;
  ostSht: number;
  etiketka: number;
  toPrintSticker: number;
}

export class IdQty {
  id: number;
  qty: number;

  constructor(id: number, qty: number) {
    this.id = id;
    this.qty = qty;
  }

  toJSON(): string {
    return JSON.stringify({id: this.id, qty: this.qty});
  }
}