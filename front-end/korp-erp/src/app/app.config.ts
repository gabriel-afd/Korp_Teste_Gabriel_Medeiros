import { ApplicationConfig, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withFetch, withInterceptors } from '@angular/common/http';
import { MatPaginatorIntl } from '@angular/material/paginator';
import { errorInterceptor } from './core/interceptors/error';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { getPtBrPaginatorIntl } from './shared/mat-paginator-intl';



import { routes } from './app.routes';
import { provideClientHydration, withEventReplay } from '@angular/platform-browser';

export const appConfig: ApplicationConfig = {
  providers: [provideZoneChangeDetection({ eventCoalescing: true }), provideRouter(routes), provideClientHydration(withEventReplay()), provideHttpClient(withFetch(), withInterceptors([errorInterceptor])), provideAnimationsAsync(), { provide: MatPaginatorIntl, useFactory: getPtBrPaginatorIntl }]
};
