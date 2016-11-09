import { NgModule, ModuleWithProviders } from '@angular/core';
//import { RequestOptions, XHRBackend } from '@angular/http';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';

import { ModalModule } from 'ng2-bootstrap/components/modal';
import { DropdownModule } from 'ng2-bootstrap/components/dropdown';

import {
  BaThemeConfig
} from './theme.config';

import {
  BaThemeConfigProvider
} from './theme.configProvider';

import {
  BaAmChart,
  BaBackTop,
  BaCard,
  BaChartistChart,
  BaCheckbox,
  BaContentTop,
  BaFullCalendar,
  BaMenuItem,
  BaMenu,
  BaMsgCenter,
  BaMultiCheckbox,
  BaPageTop,
  BaPictureUploader,
  BaSidebar,
  ModalDialog,
  HtmlForm,
  FileDownloader,
  ExportTable,
  SearchableDropdownComponent,
  BootstrapPaginator,
  DataTable,
  DefaultSorter,
  Paginator,
  Selectable,
} from './components';

import { BaCardBlur } from './components/baCard/baCardBlur.directive';

import {
  BaScrollPosition,
  BaSlimScroll,
  BaThemeRun,
  DateRangePickerDirective
} from './directives';

import {
  BaAppPicturePipe,
  BaKameleonPicturePipe,
  BaProfilePicturePipe,
  FilterByPrefixPipe,
  FileSizePipe,
} from './pipes';

import {
  BaImageLoaderService,
  BaThemePreloader,
  BaThemeSpinner,
  ExportToExcelService,
//  AbpHttp
} from './services';

import {
  EmailValidator,
  EqualValidator,
  EqualPasswordsValidator
} from './validators';

const NGA_COMPONENTS = [
  BaAmChart,
  BaBackTop,
  BaCard,
  BaChartistChart,
  BaCheckbox,
  BaContentTop,
  BaFullCalendar,
  BaMenuItem,
  BaMenu,
  BaMsgCenter,
  BaMultiCheckbox,
  BaPageTop,
  BaPictureUploader,
  BaSidebar,
  ModalDialog,
  HtmlForm,
  FileDownloader,
  ExportTable,
  SearchableDropdownComponent,
  BootstrapPaginator,
  DataTable,
  DefaultSorter,
  Paginator,
  Selectable
];

const NGA_DIRECTIVES = [
  BaScrollPosition,
  BaSlimScroll,
  BaThemeRun,
  BaCardBlur,
  DateRangePickerDirective,
  EmailValidator,
  EqualValidator
];

const NGA_PIPES = [
  BaAppPicturePipe,
  BaKameleonPicturePipe,
  BaProfilePicturePipe,
  FilterByPrefixPipe,
  FileSizePipe,
];

const NGA_SERVICES = [
  BaImageLoaderService,
  BaThemePreloader,
  BaThemeSpinner,
  ExportToExcelService,
  /*{
    provide: AbpHttp,
    useFactory: (backend: XHRBackend, options: RequestOptions) => {
      return new AbpHttp(backend, options);
    },
    deps: [XHRBackend, RequestOptions]
  }*/
];

const NGA_VALIDATORS = [
  EqualPasswordsValidator
];

const THIRD_PARTY_MODULES = [
  ModalModule,
  DropdownModule,
];

@NgModule({
  declarations: [
    ...NGA_PIPES,
    ...NGA_DIRECTIVES,
    ...NGA_COMPONENTS
  ],
  imports: [
    CommonModule,
    RouterModule,
    FormsModule,
    ReactiveFormsModule,
  ...THIRD_PARTY_MODULES
  ],
  exports: [
    ...NGA_PIPES,
    ...NGA_DIRECTIVES,
    ...NGA_COMPONENTS
  ]
})
export class NgaModule {
  static forRoot(): ModuleWithProviders {
    return <ModuleWithProviders>{
      ngModule: NgaModule,
      providers: [
        BaThemeConfigProvider,
        BaThemeConfig,
        ...NGA_VALIDATORS,
        ...NGA_SERVICES
      ],
    };
  }
}
