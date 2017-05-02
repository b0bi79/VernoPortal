export interface Filial {
  id: number;
  filialNm: string;
  fNm: string;
}

export interface Proekt {
  id: number;
  filialId: number;
  glProekt: number;
  imahProekta: string;
  imahPr: string;
  zakryt: number;
  filial: Filial;
}

export interface Nomenklatura {
  id: number;
  naimenovanie: string;
  kolichestvoUpakovka: number;
  edinicyIzmereniah: string;
}