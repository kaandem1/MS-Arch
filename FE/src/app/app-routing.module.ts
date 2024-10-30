import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './components/user/login/login.component';
import { RegisterComponent } from './components/user/register/register.component';
import { HomeDashboardComponent } from './components/dashboards/home-dashboard/home-dashboard.component';
import { SearchResultsComponent } from './components/dashboards/search-results/search-results.component';
import { ResetPasswordComponent } from './components/user/reset-password/reset-password.component';
import { UserDetailsComponent } from './components/user/details/details.component';
import { ListComponent } from './components/user/list/list.component';
import { DeviceListComponent } from './components/device/device-list/device-list.component';
import { AuthGuard } from './guard/auth-guard.guard';
import { NotFoundComponent } from './not-found/not-found.component';
import { LoginGuard } from './guard/login-guard.guard';
import { LandingPageComponent } from './components/landing-page/landing-page.component';
import { DeviceDetailsComponent } from './components/device/device-details/device-details.component';
import { MyDevicesComponent } from './components/user/my-devices/my-devices.component';
 
const routes: Routes = [
 
  { path: 'home', component: HomeDashboardComponent, canActivate: [AuthGuard] },
  { path: 'login', component: LoginComponent, canActivate: [LoginGuard]},
  { path: '', component: LandingPageComponent},
  { path: 'register', component: RegisterComponent},
  { path: 'reset-password', component: ResetPasswordComponent },
  { path: 'search-results', component: SearchResultsComponent, canActivate: [AuthGuard] },
  { path: 'user-list', component: ListComponent, canActivate: [AuthGuard]},
  { path: 'all-devices', component: DeviceListComponent, canActivate: [AuthGuard]},
  { path: 'my-devices/:id', component: MyDevicesComponent, canActivate: [AuthGuard]},
  { path: 'device-details/:id', component: DeviceDetailsComponent, canActivate: [AuthGuard]},
  { path: 'user-details/:id', component: UserDetailsComponent, canActivate: [AuthGuard]},
  { path: '**', component: NotFoundComponent }
 
];
 
@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }