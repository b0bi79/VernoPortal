export class Workbook {
  sheets: Worksheet[];
}

export class Worksheet {
  name: string;
  data: any[];
  columns: Column[];
}

export class Column {
  eval: (any) => any;
  header: string;
  //key: string;
  width: number;
}