import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-item-header',
  templateUrl: './item-header.component.html',
  styleUrl: './item-header.component.scss'
})

export class ItemHeaderComponent {
    @Input() imageUrl: string = 'https://st3.depositphotos.com/9998432/13335/v/450/depositphotos_133352010-stock-illustration-default-placeholder-man-and-woman.jpg';
    @Input() title: string | undefined = '';
    @Input() subtitle: string | undefined = '';
    @Input() details: string[] = [];
  
}
