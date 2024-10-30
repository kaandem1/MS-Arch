import { Component, Input } from '@angular/core';
import { UtilityService } from '../services/Utility/utility.service';

@Component({
  selector: 'app-item-list',
  templateUrl: './item-list.component.html',
  styleUrl: './item-list.component.scss'
})
export class ItemListComponent {
    @Input() items: any[] = [];
    @Input() titleKey: string = 'title'; 
    @Input() dateKey: string = ''; 
    @Input() linkPrefix: string = ''; 
    @Input() headerKey: string = 'header';
  constructor(public utilityService: UtilityService){

  }
}

