import { AfterViewInit, Component, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { CodingGrpcServiceClient } from '../../generated/Protos/coding.client';
import { FailedSprintRequest, GetSprintRequest, Language, SolvedSprintRequest, Sprint } from '../../generated/Protos/coding';
import { DiffEditorModel, EditorComponent } from 'ngx-monaco-editor-v2';
import { ThemeService } from '../../services/theme.service';

@Component({
  selector: 'app-sprint-page',
  templateUrl: './sprint-page.component.html',
})

export class SprintPage implements AfterViewInit {

  @ViewChild(EditorComponent, { static: false }) editorComponent?: EditorComponent;

  id: string = "";
  working = true;
  solved = false;
  finalPercentage?: number;

  editorOptions = { theme: 'vs-dark', language: '' };

  code: string = '';
  sprint?: Sprint;
  createdAt?: Date;
  languages = Language;

  originalModel: DiffEditorModel = { code: '', language: '' };
  modifiedModel: DiffEditorModel = { code: this.code, language: '' };

  constructor(private service: CodingGrpcServiceClient, private route: ActivatedRoute, private themeService: ThemeService) {

  }

  ngAfterViewInit(): void {
    this.route.params.subscribe(params => {
      this.id = params['id']; // Get the ID from the route parameter

      this.service.getSprint(GetSprintRequest.create({
        id: this.id
      })).then((res => {
        this.sprint = res.response;
        this.createdAt = new Date(Number(this.sprint.createdAt!.seconds) * 1000 + this.sprint.createdAt!.nanos / 1000000);
        this.originalModel.code = this.sprint!.codeSolution;

        this.reset();
      }), (err) => {
        console.log(err);
      })
    });

    this.themeService.getTheme().subscribe(theme => {
      this.adaptTheme(theme);
    })
  }

  adaptTheme(theme: string): void {
    if (this.editorComponent) {
      this.editorComponent!.options = { theme: this.buildEditorTheme(theme), language: this.languages[this.sprint?.language ?? 0] };
      this.editorOptions.theme = this.buildEditorTheme(theme);
    }
  }

  reset(): void {
    this.code = this.sprint!.codeExercise;

    var theme = this.themeService.getCurrentTheme();
    this.editorComponent!.options = { theme: this.buildEditorTheme(theme), language: this.languages[this.sprint!.language] };
  }

  complete(): void {
    this.working = false;
    this.modifiedModel.code = this.code;

    this.finalPercentage = Number(this.calculateAccuracy(this.code, this.sprint!.codeSolution, this.sprint!.codeExercise).toFixed(0));
    this.solved = this.finalPercentage >= 80;

    if (this.solved) {
      this.service.solvedSprint(SolvedSprintRequest.create({
        id: this.id
      })).then((res => {
        this.sprint!.solvedCount += 1;
      }), (err) => {
        console.log(err);
      })
    } else {
      this.service.failedSprint(FailedSprintRequest.create({
        id: this.id
      })).then((res => {
        this.sprint!.failedCount += 1;
      }), (err) => {
        console.log(err);
      })
    }
  }

  calculateAccuracy(input: string, solution: string, exercise: string): number {
    const normalizedInput = this.normalizeCode(input);
    const normalizedSolution = this.normalizeCode(solution);
    const normalizedExercise = this.normalizeCode(exercise);
  
    const startIndex = normalizedExercise.length;
  
    const toBeTyped = normalizedSolution.substring(startIndex);
    const userInputBeyondExercise = normalizedInput.substring(startIndex);
  
    let correctChars = 0;
    const toBeTypedChars = new Map();
  
    for (const char of toBeTyped) {
      toBeTypedChars.set(char, (toBeTypedChars.get(char) || 0) + 1);
    }
  
    for (const char of userInputBeyondExercise) {
      if (toBeTypedChars.has(char) && toBeTypedChars.get(char) > 0) {
        correctChars++;
        toBeTypedChars.set(char, toBeTypedChars.get(char) - 1);
      }
    }
  
    return toBeTyped.length > 0 ? (correctChars / toBeTyped.length) * 100 : 100;
  }
  
  normalizeCode(code: string): string {
    return code.replace(/\t/g, ' ')
      .split('\n')
      .map(line => line.trim())
      .join(' ')
      .replace(/\s+/g, ' ')
      .trim();
  }

  buildEditorTheme(theme: string) {
    return `vs-${theme}`;
  }
}