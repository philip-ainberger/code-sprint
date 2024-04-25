import { NgModule } from '@angular/core';
import { NgxHeatmapCell } from './components/ngx-heatmap-cell.component';
import { NgxHeatmapLabel } from './components/ngx-heatmap-label.component';
import { NgxHeatmapMonth } from './components/ngx-heatmap-month.component';
import { GetRangePipe } from './pipes/getRange.pipe';
import { GetEndDayOfWeekTimePipe } from './pipes/getEndDayOfWeekTime.pipe';
import { NgxHeatmapCalendar } from './ngx-heatmap-calendar.component';

@NgModule({
  declarations: [
    NgxHeatmapCell,
    NgxHeatmapLabel,
    NgxHeatmapMonth,
    GetRangePipe,
    GetEndDayOfWeekTimePipe,
    NgxHeatmapCalendar
  ],
  imports: [],
  exports: [
    NgxHeatmapCell,
    NgxHeatmapLabel,
    NgxHeatmapMonth,
    GetRangePipe,
    GetEndDayOfWeekTimePipe,
    NgxHeatmapCalendar
  ]
})
export class NgxHeatmapCalendarModule { }
