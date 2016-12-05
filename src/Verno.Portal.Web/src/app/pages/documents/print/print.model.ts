export class PrintDocument implements abp.services.app.IPrintDocument {
    constructor(liniah: number, nomNakl: string, dataNakl: Date, imahDok: string, url: string) {
        this.liniah = liniah;
        this.nomNakl = nomNakl;
        this.dataNakl = dataNakl;
        this.imahDok = imahDok;
        this.url = url;
    }

    id: number;
    liniah: number;
    nomNakl: string;
    dataNakl: Date;
    imahDok: string;
    srcWarehouse: string;
    srcWhId: number;
    url: string;
}