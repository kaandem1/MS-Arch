import { Component, Input } from '@angular/core';
import { Device } from '../../../interfaces/Device';

@Component({
  selector: 'device-list-item',
  templateUrl: './device-list-item.component.html',
  styleUrls: ['./device-list-item.component.scss']
})
export class DeviceListItemComponent {
  @Input() device?: Device;
}
