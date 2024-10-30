import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router, NavigationEnd } from '@angular/router';
import { Subscription } from 'rxjs';
import { RoutingService } from '../../../services/Routing/routing.service';

@Component({
  selector: 'nav-bar',
  templateUrl: './nav-bar.component.html',
  styleUrls: ['./nav-bar.component.scss']
})
export class NavBarComponent implements OnInit, OnDestroy {
  display: boolean = false;
  displayNavBar: boolean = true;
  displaySearchBar: boolean = true;
  searchTerm: string = '';
  trendingData: any;
  mostWantedData: any;
  private routerSubscription: Subscription = new Subscription();

  constructor(
    private router: Router,
    private routingService: RoutingService
  ) {}

  ngOnInit(): void {
    this.routingService.displayNavBar$.subscribe(display => this.displayNavBar = display);
    this.routerSubscription = this.router.events.subscribe(event => {
      if (event instanceof NavigationEnd) {
        this.display = false;
      }
    });
  }

  ngOnDestroy(): void {
    if (this.routerSubscription) {
      this.routerSubscription.unsubscribe();
    }
  }

  onSearch(): void {
    if (this.searchTerm) {
      this.router.navigate(['/search-results', { name: this.searchTerm }]);
    }
  }

}
