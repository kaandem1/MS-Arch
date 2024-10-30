import { Component, OnInit } from '@angular/core';
import { Device } from '../../../interfaces/Device';
import { DeviceService } from '../../../services/Device/device.service';

@Component({
  selector: 'device-list',
  templateUrl: './device-list.component.html',
  styleUrls: ['./device-list.component.scss']
})
export class DeviceListComponent implements OnInit {
  devices: Device[] = [];
  filteredDevices: Device[] = [];
  error: string | null = null;
  maxHourlyConsumptionFilter: number | null = null;

  constructor(private deviceService: DeviceService) {}

  ngOnInit(): void {
    this.loadDevices();
  }

  loadDevices(): void {
    this.deviceService.getAllDevices().subscribe({
      next: (data: Device[]) => {
        this.devices = data;
        this.filteredDevices = data;
      },
      error: (err) => {
        this.error = 'Failed to load devices: ' + err.message;
      }
    });
  }

  filterDevices(): void {
    if (this.maxHourlyConsumptionFilter != null) {
      this.filteredDevices = this.devices.filter(device => 
        device.maxHourlyCons <= this.maxHourlyConsumptionFilter!
      );
    } else {
      this.filteredDevices = this.devices; 
    }
  }
  
}
