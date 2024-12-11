import { CUSTOM_ELEMENTS_SCHEMA, NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';

import { AppRoutingModule } from './app-routing.module';

import { RegisterComponent } from './components/user/register/register.component';
import { AppComponent } from './app.component';
import { ResetPasswordComponent } from './components/user/reset-password/reset-password.component';
import { LoginComponent } from './components/user/login/login.component';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';


import { UserDetailsComponent } from './components/user/details/details.component';
import { DeviceListComponent } from './components/device/device-list/device-list.component';
import { ListComponent } from './components/user/list/list.component';
import { HomeDashboardComponent } from './components/dashboards/home-dashboard/home-dashboard.component';
import { HomeMenuComponent } from './components/menus/home-menu/home-menu.component';
import { SongCardComponent } from './components/dashboards/song-card/song-card.component';
import { NavBarComponent } from './components/dashboards/nav-bar/nav-bar.component';
import { SearchResultsComponent } from './components/dashboards/search-results/search-results.component';


import { SidebarModule } from 'primeng/sidebar';
import { ButtonModule } from 'primeng/button';
import { PanelModule } from 'primeng/panel';
import { DividerModule } from 'primeng/divider';
import { InputTextModule } from 'primeng/inputtext';
import { InputNumberModule } from 'primeng/inputnumber';
import { CalendarModule } from 'primeng/calendar';
import { MessageModule } from 'primeng/message';
import { MessagesModule } from 'primeng/messages';
import { DataViewModule } from 'primeng/dataview';
import { CardModule } from 'primeng/card';
import { ListboxModule } from 'primeng/listbox';
import { ChipModule } from 'primeng/chip';


import { UniversalAppInterceptor } from './interceptors/universal-app.interceptor';
import { AppCookieService } from './services/AppCookie/app-cookie.service';
import { AuthService } from './services/Authorization/auth.service';
import { JWTTokenService } from './services/JWTToken/jwttoken.service';

import { AuthGuard } from './guard/auth-guard.guard';

import { TitleComponent } from './components/title/title.component';
import { LoadingComponent } from './components/loading/loading.component';

import { ItemListComponent } from './item-list/item-list.component';
import { ItemHeaderComponent } from './item-header/item-header.component';
import { ItemSearchComponent } from './item-search/item-search.component';

import { DialogModule } from 'primeng/dialog';
import { ToggleButtonModule } from 'primeng/togglebutton';
import { PickListModule } from 'primeng/picklist';
import { OrderListModule } from 'primeng/orderlist';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { TimelineModule } from 'primeng/timeline';
import { StepsModule } from 'primeng/steps';
import { FloatLabelModule } from 'primeng/floatlabel';
import { ItemGridComponent } from './components/items/item-grid/item-grid.component';
import { CustomScrollbarDirective } from './custom-scrollbar.directive';

import { ToastModule } from 'primeng/toast';
import { MessageService } from 'primeng/api';
import { LandingPageComponent } from './components/landing-page/landing-page.component';
import { ThemeToggleComponent } from './theme-toggle/theme-toggle.component';
import { NotFoundComponent } from './not-found/not-found.component';
import { DeviceListItemComponent } from './components/items/device-list-item/device-list-item.component';
import { DeviceDetailsComponent } from './components/device/device-details/device-details.component';
import { UserDeviceListComponent } from './components/device/user-device-list/user-device-list.component';
import { MyDevicesComponent } from './components/user/my-devices/my-devices.component';
import { NgChartsModule } from 'ng2-charts';



@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    ResetPasswordComponent,
    UserDetailsComponent,
    ListComponent,
    RegisterComponent,
    HomeDashboardComponent,
    HomeMenuComponent,
    SongCardComponent,
    NavBarComponent,
    TitleComponent,
    LoadingComponent,

    SearchResultsComponent,  
    ItemGridComponent, 
    LandingPageComponent,    
    ItemListComponent,
    ItemHeaderComponent,
    ItemSearchComponent,
    CustomScrollbarDirective,
    ThemeToggleComponent,
    NotFoundComponent,
    DeviceListComponent,
    DeviceListItemComponent,
    DeviceDetailsComponent,
    UserDeviceListComponent,
    MyDevicesComponent,

    
  ],
  imports: [
    FloatLabelModule,
    StepsModule,
    TimelineModule,
    DialogModule,
    PickListModule,
    OrderListModule,
    PanelModule,
    DividerModule,
    ButtonModule,
    SidebarModule,
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    ReactiveFormsModule,
    CommonModule,
    FormsModule,
    ConfirmDialogModule,
    ToggleButtonModule,
    BrowserModule,
    BrowserAnimationsModule,
    FormsModule,
    ReactiveFormsModule,
    HttpClientModule,
    CommonModule,
    AppRoutingModule,
    MatInputModule,
    MatButtonModule,
    MatFormFieldModule,
    SidebarModule,
    ButtonModule,
    PanelModule,
    DividerModule,
    InputTextModule,
    InputNumberModule,
    CalendarModule,
    ToastModule,
    MessagesModule,
    MessageModule,
    DataViewModule,
    CardModule,
    ListboxModule,
    ChipModule,
    NgChartsModule
    
  ],
  providers: [
    AuthService,
    AuthGuard,
    JWTTokenService,
    MessageService,
    AppCookieService,
    {
      provide: HTTP_INTERCEPTORS,
      useClass: UniversalAppInterceptor,
      multi: true
    },
    provideAnimationsAsync(),
  ],
  bootstrap: [AppComponent],
  schemas: [CUSTOM_ELEMENTS_SCHEMA]
})
export class AppModule { }
