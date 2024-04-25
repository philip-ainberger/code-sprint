import { AfterViewInit, Component, OnInit, QueryList, ViewChildren } from '@angular/core';
import { CodingGrpcServiceClient } from '../../generated/Protos/coding.client';
import { CreateSprintRequest, GetSprintRequest, Language, UpdateSprintRequest } from '../../generated/Protos/coding';
import { DiffEditorModel, EditorComponent } from 'ngx-monaco-editor-v2';
import { ActivatedRoute, Router } from '@angular/router';
import { TaggingGrpcServiceClient } from '../../generated/Protos/tagging.client';
import { ListTagsRequest, Tag } from '../../generated/Protos/tagging';
import { FormControl } from '@angular/forms';
import { ThemeService } from '../../services/theme.service';

@Component({
  selector: 'app-sprint-edit',
  templateUrl: './sprint-edit.component.html',
})
export class SprintEditPage implements OnInit, AfterViewInit {
  @ViewChildren(EditorComponent) editorComponents?: QueryList<EditorComponent>;
  
  id: string = "";
  tags: Tag[];
  sprint: CreateSprintRequest | UpdateSprintRequest;
  languageKeys = Object.keys(Language).filter(item => !isNaN(Number(item))).map(Number);
  languages = Language;
  addingSprint = false;
  updatingSprint = false;

  editorOptions = { theme: 'vs-dark', language: 'csharp' };

  solution = "";
  originalModel: DiffEditorModel = {
    code: this.solution,
    language: 'csharp'
  };

  modifiedModel: DiffEditorModel = {
    code: '',
    language: 'csharp'
  };

  selectControl = new FormControl();

  constructor(
    private service: CodingGrpcServiceClient,
    private tagginService: TaggingGrpcServiceClient,
    private route: ActivatedRoute,
    private router: Router,
    private themeService: ThemeService
  ) {
    this.sprint = CreateSprintRequest.create();
    this.tags = [];

    this.tagginService.listTags(ListTagsRequest.create())
      .then(res => {
        this.tags = res.response.tags;
      },
        err => {
          console.log(err);
        }
      );
  }
  ngAfterViewInit(): void {
    this.adaptTheme(this.themeService.getCurrentTheme());
  }

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      this.id = params['id']; // Get the ID from the route parameter

      if (this.id != undefined) {
        this.service.getSprint(GetSprintRequest.create({
          id: this.id
        })).then((res => {
          this.sprint = UpdateSprintRequest.create({
            id: res.response.id,
            codeExercise: res.response.codeExercise,
            codeSolution: res.response.codeSolution,
            description: res.response.description,
            language: res.response.language,
            title: res.response.title,
            tags: res.response.tags.map(c => c.id)
          });
          
          var languageSelect = this.languageSelectElement();
          languageSelect.value = this.sprint.language.toString();
          
          var tagsSelect = this.tagsSelectElement();
          for (let index = 0; index < tagsSelect.options.length; index++) {
            var option = tagsSelect.options[index];
            
            if (this.sprint.tags.includes(option.value)) {
              option.selected = true;
            }
          }

        }), (err) => {
          console.log(err);
        });
      }
    });

    this.themeService.getTheme().subscribe(theme => {
      this.adaptTheme(theme);
    });
  }
  
  adaptTheme(theme: string): void {
    if (this.editorComponents) {
      this.editorComponents.forEach(editor => {
        editor!.options = { theme: this.buildEditorTheme(theme), language: this.languages[+this.languageSelectElement().value] };
        this.editorOptions.theme = this.buildEditorTheme(theme);
      })
    }
  }

  buildEditorTheme(theme: string) {
    return `vs-${theme}`;
  }

  updateLanguage() {
    this.adaptTheme(this.themeService.getCurrentTheme());
  }

  languageSelectElement(): HTMLSelectElement {
    return document.getElementById("language") as HTMLSelectElement;
  }

  tagsSelectElement(): HTMLSelectElement {
    return document.getElementById("tags") as HTMLSelectElement;
  }

  mapRequest(): void {
    this.sprint.language = +this.languageSelectElement().value;

    var tags: string[] = [];
    var tagsElement = this.tagsSelectElement();

    for (let index = 0; index < tagsElement.options.length; index++) {
      var option = tagsElement.options[index];
      if (option.selected) {
        tags.push(option.value);
      }
    }

    this.sprint.tags = tags;
  }

  add(): void {
    this.addingSprint = true;
    
    this.mapRequest();

    this.service.createSprint(this.sprint as CreateSprintRequest).then((res) => {
      this.router.navigate(['sprints']);
    }, (err) => {
      console.log(err);
      this.addingSprint = false;
    });
  }

  update(): void {
    this.updatingSprint = true;
    
    this.mapRequest();

    this.service.updateSprint(this.sprint as UpdateSprintRequest).then((res) => {
      this.router.navigate(['sprints']);
    }, (err) => {
      console.log(err);
      this.addingSprint = false;
    });
  }
}
