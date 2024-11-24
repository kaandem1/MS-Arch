import { Injectable } from '@angular/core';
import { Router, NavigationEnd } from '@angular/router';
import { BehaviorSubject } from 'rxjs';
import { filter } from 'rxjs/operators';
 
@Injectable({
  providedIn: 'root'
})
export class RoutingService {
 
  private displayNavBarSubject = new BehaviorSubject<boolean>(true);
  displayNavBar$ = this.displayNavBarSubject.asObservable();

  private routesWithNavBar: string[] = [
    '/home',  '/search-results', 
    '/user-list', '/user-details', '/all-devices', 
    '/device-details', '/my-devices'
  ];
 
  constructor(private router: Router) {
    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd)
    ).subscribe(() => {
      this.updateNavBarVisibility();
    });
 
    this.updateNavBarVisibility();
  }
 
  private updateNavBarVisibility(): void {
    const currentUrl = this.router.url.split('?')[0];
    const shouldDisplayNavBar = this.routesWithNavBar.some(route => currentUrl.includes(route));
    this.displayNavBarSubject.next(shouldDisplayNavBar);
  }
}