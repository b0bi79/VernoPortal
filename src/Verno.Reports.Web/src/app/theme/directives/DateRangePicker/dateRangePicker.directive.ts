import { Directive, ElementRef, OnInit, Input, Output, EventEmitter } from '@angular/core';
import * as moment from 'moment';

@Directive({
  selector: '[daterangepicker]'
})

export class DateRangePickerDirective implements OnInit {
  @Input() options: Object = {};
  @Output() selected: EventEmitter<any> = new EventEmitter();

  constructor(private elementRef: ElementRef) { }

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
    jQuery(this.elementRef.nativeElement)
      .daterangepicker(this.options, this.dateCallback.bind(this));
  }

  dateCallback(start, end, label) {
    this.selected.emit({ start, end });
  }
}