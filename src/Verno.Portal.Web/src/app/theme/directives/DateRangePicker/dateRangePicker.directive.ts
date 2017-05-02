import { Directive, ElementRef, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { NgModel } from '@angular/forms';
import * as moment from 'moment';

@Directive({
  selector: '[daterangepicker]'
})

export class DateRangePickerDirective implements OnInit {
  @Input() options: Object = {};
  @Output() selected: EventEmitter<any> = new EventEmitter();
  singleDatePicker: boolean;

  constructor(private elementRef: ElementRef, private ngModel: NgModel) { }

  ngOnInit() {
    this.options["locale"] = {
      "format": "DD.MM.YYYY",
      "separator": " - ",
      "applyLabel": "Принять",
      "cancelLabel": "Отмена",
      "fromLabel": "От",
      "toLabel": "До",
      "customRangeLabel": "Выбор",
      "weekLabel": "Н",
      "daysOfWeek": ["Вс", "Пн", "Вт", "Ср", "Чт", "Пт", "Сб"],
      "monthNames": ["Январь", "Февраль", "Март", "Апрель", "Май", "Июнь", "Июль", "Август", "Сентябрь", "Октябрь", "Ноябрь", "Декабрь"],
      "firstDay": 1
    };
    this.options['ranges'] = {
      'Сегодня': [moment(), moment()],
      'Вчера': [moment().subtract(1, 'days'), moment().subtract(1, 'days')],
      'Последние 7 дней': [moment().subtract(6, 'days'), moment()],
      'Последние 30 дней': [moment().subtract(29, 'days'), moment()],
      'Этот месяц': [moment().startOf('month'), moment().endOf('month')],
      'Прошлый месяц': [moment().subtract(1, 'month').startOf('month'), moment().subtract(1, 'month').endOf('month')]
    };
    this.singleDatePicker = this.options["singleDatePicker"];
    jQuery(this.elementRef.nativeElement)
      .daterangepicker(this.options, this.dateCallback.bind(this));

    let that = this;
    that.ngModel.valueChanges.subscribe(value => {
      var picker = jQuery(this.elementRef.nativeElement).data('daterangepicker');
      if (this.singleDatePicker) {
        picker.setStartDate(value);
      } else {
        picker.setStartDate(value.start);
        picker.setEndDate(value.end);
      }
    });
  }

  dateCallback(start, end, label) {
    let value = this.singleDatePicker ? start : { start: start, end: end };
    this.ngModel.update.emit(value);
    this.selected.emit(value);
  }
}