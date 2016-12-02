import { NgModule }      from '@angular/core';
import { CommonModule }  from '@angular/common';
import { FormsModule } from '@angular/forms';
import { NgaModule } from '../../theme/nga.module';

import { Reports } from './reports.component';
import { routing }       from './reports.routing';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    NgaModule,
    CommonModule,
    NgaModule,
    routing
  ],
  declarations: [
    /*PopularApp,
    PieChart,
    TrafficChart,
    UsersMap,
    LineChart,
    Feed,
    Calendar,*/
    Reports
  ],
  providers: [
    /*CalendarService,
    FeedService,
    LineChartService,
    PieChartService,
    TodoService,
    TrafficChartService,
    UsersMapService*/
  ]
})
export default class DashboardModule {}
