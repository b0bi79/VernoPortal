import { Component, ViewEncapsulation, OnInit, ElementRef } from '@angular/core';
import { ActivatedRoute, Params } from '@angular/router';
import { Location } from '@angular/common';

import * as moment from 'moment';
import { LocalDataSource } from 'ng2-smart-table';

import { ProizvMagSpecDto, ProizvMagSpec, ProizvMagSpecItemDto, ProizvMagSpecItem } from "../../production-spec.model";
import { ProductionSpecService } from "../../production-spec.service";
import { Proekt, InfoService, Nomenklatura } from 'app/theme/services/shInfoSql';

@Component({
  selector: 'spec-edit',
  encapsulation: ViewEncapsulation.None,
  template: require('./spec-edit.html'),
  providers: [ProductionSpecService, InfoService]
})
export class SpecEdit {
  private isNew: boolean = true;
  private title: string;

  private proektNames: Array<string> = [];
  private proekts: Array<Proekt> = [];

  private data: ProizvMagSpecDto;
  private items: LocalDataSource;
  private tovars: Nomenklatura[];

  settings = {
    add: {
      addButtonContent: '<i class="ion-ios-plus-outline"></i>',
      createButtonContent: '<i class="ion-checkmark"></i>',
      cancelButtonContent: '<i class="ion-close"></i>',
    },
    edit: {
      editButtonContent: '<i class="ion-edit"></i>',
      saveButtonContent: '<i class="ion-checkmark"></i>',
      cancelButtonContent: '<i class="ion-close"></i>',
    },
    delete: {
      deleteButtonContent: '<i class="ion-trash-a"></i>',
      confirmDelete: true
    },
    columns: {
/*
  vidTovara: number;
  nameTov: number;
  norm?: number;
  koeff?: number;
*/
      id: {
        title: '#',
        editable: false,
        addable: false,
      },
      tovar: {
        title: 'Товар',
        editor: {
          type: 'completer',
          config: {
            completer: {
              data: [],//()=>this.tovars,
              searchFields: 'naimenovanie',
              titleField: 'naimenovanie',
              descriptionField: 'naimenovanie',
            },
          },
        },
      },
      norm: {
        title: 'Норматив',
        type: 'number'
      },
      koeff: {
        title: 'Коэффициент',
        type: 'number'
      },
    },
  };

  pickerOptions: Object = {
    'singleDatePicker': true,
    'showWeekNumbers': true,
    'startDate': moment(),
    "minDate": moment(),
  };

  constructor(
    private service: ProductionSpecService,
    private infoService: InfoService,
    private route: ActivatedRoute,
    private location: Location,
    private element: ElementRef
  ) { }

  ngOnInit(): void {
    var self = this;

    this.spec = new ProizvMagSpec();
    this.items = new LocalDataSource();

    this.route.params.forEach((params: Params) => {
      let id = +params['id'];
      if (id)
        this.setBusy(this.service.get(id).then(s => {
          this.spec = s;
          this.setBusy(this.service.getItems(id).then(result => this.items.load(result.items)));
        }));
      else
        this.isNew = true;
    });

    this.setBusy(
      this.infoService.getProekts().then(result => {
        self.proektNames = result.items.map(x => x.imahProekta);
        self.proekts = result.items;
      }));
  }

  set spec(value: ProizvMagSpecDto) {
    this.isNew = value == null;
    this.title = this.isNew ? "Новая спецификация" : "Редактирование спецификации";
    this.data = value;
  }
  get spec(): ProizvMagSpecDto { return this.data; }

  save(value: ProizvMagSpecDto, isValid: boolean) {
    if (!isValid)
      return;

    this.setBusy(this.service.save(this.spec).then(() => this.goBack()));
  }

  private setBusy(promise: any): void {
    abp.ui.setBusy(jQuery('form', this.element.nativeElement),
      {
        blockUI: true,
        promise: promise
      });
  }

  private goBack(): void {
    this.location.back();
  }

  private proektSelected(value: any): void {
    var idx = this.proektNames.indexOf(value);
    this.spec.proektName = value;
    this.spec.proekt = this.proekts[idx].id;
    var self = this;
    this.service.getNames(this.spec.proekt).then(result => {
      self.settings.columns.tovar.editor.config.completer.data = result.items;
//      self.tovars = result.items;
    });
  }

  onDeleteConfirm(event): void {
    if (window.confirm('Are you sure you want to delete?')) {
      event.confirm.resolve();
    } else {
      event.confirm.reject();
    }
  }
}