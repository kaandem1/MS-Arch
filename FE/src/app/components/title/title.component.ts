import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router, NavigationEnd } from '@angular/router';
import { Subscription } from 'rxjs';
import { filter } from 'rxjs/operators';

@Component({
  selector: 'app-title',
  templateUrl: './title.component.html',
  styleUrls: ['./title.component.scss']
})
export class TitleComponent implements OnInit, OnDestroy {
  title?:string;
  private routerSub: Subscription = new Subscription();

  constructor(private router: Router) {}

  ngOnInit(): void {
    this.routerSub = this.router.events.pipe(
      filter(event => event instanceof NavigationEnd)
    ).subscribe(() => {
      this.updateTitle();
    });
    this.updateTitle();
  }

  ngOnDestroy(): void {
    this.routerSub.unsubscribe();
  }

  private updateTitle(): void {
    const currentUrl = this.router.url;
  
    switch (currentUrl) {
      case '/all-devices':
        this.title = 'Devices';
        break;
      case '/user-list':
        this.title = 'User List';
        break;
      case '/home':
        this.title ='Check this out!'
        break;
      default:
        this.title = '';
        break;
    }

  }
  
}
