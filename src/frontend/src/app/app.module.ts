import { APP_INITIALIZER, NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AppComponent } from './root/app.component';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AppRoutingModule } from './app-routing.module';
import { LayoutPage } from './pages/layout/layout.component';
import { SprintListPage } from './pages/sprint-list/sprint-list.component';
import { ImportIcons } from './setup/IconImports';
import { ConfigurationService } from './services/configuration.service';
import { AuthService } from './services/auth.service';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { AuthInterceptor } from './core/auth.interceptor';
import { SprintPage } from './pages/sprint-page/sprint-page.component';
import { SprintEditPage } from './pages/sprint-edit/sprint-edit.component';
import { TagListPage } from './pages/tag-list/tag-list.component';
import { LoginPage } from './pages/login/login.component';
import { AnimatedNumberComponent } from './components/animated-number/animated-number.component';
import { CodingGrpcServiceClient } from './generated/Protos/coding.client';
import { GrpcWebFetchTransport } from '@protobuf-ts/grpcweb-transport';
import { TaggingGrpcServiceClient } from './generated/Protos/tagging.client';
import { CustomSelectComponent } from './components/custom-select/custom-select.component';
import { MonacoEditorModule } from 'ngx-monaco-editor-v2';
import { CustomBadgeComponent } from './components/custom-badge/custom-badge.component';
import { GrpcAuthInterceptor } from './core/grpc.auth.interceptor';
import { NgxHeatmapCalendarModule } from './components/heatmap-calendar/ngx-heatmap-calendar.module';
import { TypingAnimationDirective } from './core/typing.animation.directive';

export function initializeApp(configService: ConfigurationService, authService: AuthService) {
    return async (): Promise<void> => {
        await configService.loadConfig();
        authService.validateToken();
    };
}

@NgModule({
    declarations: [
        AppComponent,
        LayoutPage,
        SprintListPage,
        SprintPage,
        SprintEditPage,
        TagListPage,
        LoginPage,
        AnimatedNumberComponent,
        TypingAnimationDirective
    ],
    imports: [
        BrowserModule,
        AppRoutingModule,
        CommonModule,
        FormsModule,
        HttpClientModule,
        CustomSelectComponent,
        CustomBadgeComponent,
        NgxHeatmapCalendarModule,
        MonacoEditorModule.forRoot(),
        ImportIcons()
    ],
    exports: [],
    providers: [
        { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true },
        ConfigurationService,
        AuthService,
        {
            provide: APP_INITIALIZER,
            useFactory: initializeApp,
            deps: [ConfigurationService, AuthService],
            multi: true,
        },
        {
            provide: CodingGrpcServiceClient,
            useFactory: (configService: ConfigurationService, authService: AuthService) => {
                const transport = new GrpcWebFetchTransport({
                    baseUrl: configService.getApiBaseUrl(),
                    interceptors: [GrpcAuthInterceptor(authService.getToken().value!)]
                });
                
                const client = new CodingGrpcServiceClient(transport);
                return client;
            },
            deps: [ConfigurationService, AuthService]
        },
        {
            provide: TaggingGrpcServiceClient,
            useFactory: (configService: ConfigurationService, authService: AuthService) => {
                const transport = new GrpcWebFetchTransport({
                    baseUrl: configService.getApiBaseUrl(),
                    interceptors: [GrpcAuthInterceptor(authService.getToken().value!)]
                });

                const client = new TaggingGrpcServiceClient(transport);
                return client;
            },
            deps: [ConfigurationService, AuthService]
        }
    ],
    bootstrap: [AppComponent]
})
export class AppModule { }