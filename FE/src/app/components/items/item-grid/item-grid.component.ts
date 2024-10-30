import { Component, Input } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'item-grid',
  templateUrl: './item-grid.component.html',
  styleUrl: './item-grid.component.scss'
})
export class ItemGridComponent {
  @Input() items: any[] = [];
  @Input() type: string = '';

  constructor(private router: Router) {}


  getItemName(item: any): string {
    switch (this.type) {
      case 'devices':
        return item.deviceName;
      default:
        return '';
    }
  }

  getRouterLink(item: any): string[] {
    switch (this.type) {
      case 'devices':
        return ['/device-details', item.id];
      default:
        return ['/login'];
    }
  }

  getImageUrl(item: any): string {
    switch (this.type) {
      case 'devices':
        return '/assets/computer_small.png';
      default:
        return '/assets/computer_small.png';
    }
  }
}
