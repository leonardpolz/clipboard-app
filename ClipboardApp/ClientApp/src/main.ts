import { enableProdMode, importProvidersFrom } from '@angular/core';
import { environment } from './environments/environment';
import {HTTP_INTERCEPTORS, HttpClientModule} from '@angular/common/http';
import { BrowserModule, bootstrapApplication } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { RouterModule } from '@angular/router';
import { routes } from './app/app-routing';
import { AppComponent } from './app/app.component';
import { API_BASE_URL, AuthGuestClient, BinaryFileClient, TextClient } from './app/shared/services/backend.client';
import { AuthService } from './app/shared/services/auth-service/auth.service';
import { TokenInterceptorInterceptor } from './app/shared/interceptors/token-interceptor/token-interceptor.interceptor';
import { WsTextClient } from './app/shared/services/text-ws.client';
import { BinaryFileClientCustom } from './app/shared/services/upload-file.client';
import { WsFileNameClient } from './app/shared/services/fileName-ws-client';

export function getBaseUrl() {
  return document.getElementsByTagName('base')[0].href;
}

const imports = [
  RouterModule.forRoot(routes),
  BrowserModule.withServerTransition({ appId: 'serverApp' }),
  HttpClientModule,
  BrowserAnimationsModule
];

const providers = [
  { provide: 'BASE_URL', useFactory: getBaseUrl, deps: [] },
  {
    provide: API_BASE_URL,
    useValue: environment.apiRoot
  },
  {
    provide: HTTP_INTERCEPTORS,
    useClass: TokenInterceptorInterceptor,
    multi: true
  },
  TextClient,
  WsTextClient,
  BinaryFileClientCustom,
  AuthGuestClient,
  AuthService,
  WsFileNameClient,
  importProvidersFrom(imports)
];


if (environment.production)
  enableProdMode();

bootstrapApplication(AppComponent, {
    providers: providers,
}).catch(err => console.error(err));
