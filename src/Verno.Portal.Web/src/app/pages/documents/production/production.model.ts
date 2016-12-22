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
}