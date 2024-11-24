import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-item-search',
  templateUrl: './item-search.component.html',
  styleUrl: './item-search.component.scss'
})
export class ItemSearchComponent {
  @Input() placeholder: string = 'Search...';
  @Input() searchTerm: string = '';
  @Output() search = new EventEmitter<string>();

  onSearch() {
    this.search.emit(this.searchTerm);
  }
}
