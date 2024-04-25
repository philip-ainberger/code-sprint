import { Component, OnInit } from '@angular/core';
import { CodingGrpcServiceClient } from '../../generated/Protos/coding.client';
import { TaggingGrpcServiceClient } from '../../generated/Protos/tagging.client';
import { Router } from '@angular/router';
import { DeleteSprintRequest, GetCodingActivityRequest, Language, ListSprintsRequest, Sprint } from '../../generated/Protos/coding';
import { ListTagsRequest, Tag } from '../../generated/Protos/tagging';
import { HeatMapDate } from '../../components/heatmap-calendar/ngx-heatmap-calendar.interface';

@Component({
  selector: 'app-sprint-list',
  templateUrl: './sprint-list.component.html'
})
export class SprintListPage implements OnInit {

  sprints: Sprint[];
  tags: Tag[] = [];
  selectedSprint?: Sprint;

  totalSprints = 0;
  currentPage = 0;
  maxPage = 0;
  defaultPageSize = 10;

  languageKeys = Object.keys(Language).filter(item => !isNaN(Number(item))).map(Number);
  languages = Language;

  startDate = new Date(2024, 0, 1);
  endDate = new Date(2024, 11, 31);

  dates: HeatMapDate[] = [];

  callBackCssClass = ({ date, value }: HeatMapDate) => {
    if((date).setUTCHours(0, 0, 0, 0) === new Date(Date.now()).setUTCHours(0, 0, 0, 0)) {
      return 'fill-light-l3 dark:fill-dark-l2/50';
    }
    
    if(value! >= 9) return 'fill-value-4';
    if(value! >= 6) return 'fill-value-3';
    if(value! >= 3) return 'fill-value-2';
    if(value! >= 1) return 'fill-value-1';

    return 'fill-light-l3/60 dark:fill-dark-l3/70';
  };

  constructor(
    private service: CodingGrpcServiceClient,
    private taggingService: TaggingGrpcServiceClient,
    private router: Router) {
    
      this.sprints = [];
  }

  ngOnInit(): void {
    this.fetchData();

    this.taggingService.listTags(ListTagsRequest.create())
      .then(res => {
        this.tags = res.response.tags;
      },
        err => {
          console.log(err);
        }
      );
  }

  filterChanged(): void {
    this.fetchData();
  }

  newSprint(): void {
    this.router.navigate(["/new-sprint"]);
  }

  languagesSelectElement(): HTMLSelectElement {
    return document.getElementById("languages") as HTMLSelectElement;
  }

  tagsSelectElement(): HTMLSelectElement {
    return document.getElementById("tags") as HTMLSelectElement;
  }

  fetchData(): void {
    var request = ListSprintsRequest.create({
      page: this.currentPage,
      filter: {
        tags: [],
        languages: []
      }
    });

    var languagesSelect = this.languagesSelectElement();
    for (let index = 0; index < languagesSelect?.options.length; index++) {
      var option = languagesSelect.options[index];
      
      if (option.selected) {
        request.filter!.languages.push(+option.value);
      }
    }

    var tagsSelect = this.tagsSelectElement();
    for (let index = 0; index < tagsSelect?.options.length; index++) {
      var option = tagsSelect.options[index];
      
      if (option.selected) {
        request.filter!.tags.push(option.value);
      }
    }

    this.service.listSprints(request).then((res) => {
      this.sprints = res.response.sprints;
      this.totalSprints = res.response.totalCount;

      this.maxPage = Math.ceil(this.totalSprints / this.defaultPageSize);
    });

    this.service.getCodingActivity(GetCodingActivityRequest.create()).then((res) => {
      this.dates = [];

      res.response.activities.forEach(activity => {
        var date = new Date(Number(activity.timestamp!.seconds) * 1000 + activity.timestamp!.nanos / 1000000);
        this.dates.push({ date, value: activity.count })
      });
    });
  }

  deleteSprint(id: string): void {
    this.service.deleteSprint(DeleteSprintRequest.create({
      id: id
    })).then((res) => {
      this.fetchData();
    });
  }

  nextPage(): void{
    this.loadPage(this.currentPage+1);
  }
  
  previousPage(): void{
    this.loadPage(this.currentPage-1);
  }
  
  loadPage(pageIndex: number): void {
    if(pageIndex >= this.maxPage || pageIndex < 0) {
      return;
    }
    this.currentPage = pageIndex;
    this.fetchData();
  }

  getDescription(text: string): string {
    if(text.length > 45)
      return text.substring(0, 45) + " ..."

    return text;
  }
}