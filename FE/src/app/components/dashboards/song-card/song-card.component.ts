import { Component, Input,Injectable } from '@angular/core';

@Component({
  selector: 'song-card',
  templateUrl: './song-card.component.html',
  styleUrl: './song-card.component.scss'
})
export class SongCardComponent {
  @Input() songName: string ="" ;
  @Input() artistName: string ="";
 
}
