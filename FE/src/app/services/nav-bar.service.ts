import { Injectable } from '@angular/core';
import { Router, NavigationEnd } from '@angular/router';
import { BehaviorSubject } from 'rxjs';
import { filter } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class NavbarService {
  private displayNavBarSubject = new BehaviorSubject<boolean>(true);
  private displaySearchBarSubject = new BehaviorSubject<boolean>(true);

  displayNavBar$ = this.displayNavBarSubject.asObservable();
  displaySearchBar$ = this.displaySearchBarSubject.asObservable();

  constructor(private router: Router) {
    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd)
    ).subscribe(() => {
      this.updateNavBarVisibility();
    });

    this.updateNavBarVisibility();
  }

  private updateNavBarVisibility(): void {
    const currentUrl = this.router.url;
    const shouldDisplayNavBar = !(currentUrl === '/' || currentUrl === '/reset-password');
    this.displayNavBarSubject.next(shouldDisplayNavBar);
  }

}
