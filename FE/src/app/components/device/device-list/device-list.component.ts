import { Component, OnInit } from '@angular/core';
import { Device } from '../../../interfaces/Device';
import { DeviceService } from '../../../services/Device/device.service';
import { DeviceCreate } from '../../../interfaces/DeviceCreate';
import { MessageService } from 'primeng/api';
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
  showCreationForm: boolean = false;
  newDevice: DeviceCreate = { 
    deviceName: '',
    description: '',
    address: '',
    maxHourlyCons: 0
  };

  constructor(private deviceService: DeviceService, private messageService: MessageService) {}

  ngOnInit(): void {
    this.loadDevices();
  }

  toggleCreationForm(): void {
    this.showCreationForm = !this.showCreationForm;
    if (!this.showCreationForm) {
      this.resetNewDevice();
    }
  }

  createDevice(formValue: any): void {
    const deviceToCreate: DeviceCreate = {
      deviceName: formValue.DeviceName,
      description: formValue.Description,
      address: formValue.Address,
      maxHourlyCons: formValue.MaxHourlyCons
    };

    this.deviceService.createDevice(deviceToCreate).subscribe({
      next: (newDevice: Device) => {
        this.devices.push(newDevice);
        this.filteredDevices.push(newDevice);
        this.showToast('success', 'Device created successfully.');
        this.resetNewDevice();
        this.toggleCreationForm();
      },
      error: (err) => {
        this.error = 'Failed to create device: ' + err.message;
        this.showToast('error', 'Failed to create device.');
      }
    });
  }

  resetNewDevice(): void {
    this.newDevice = {
      deviceName: '',
      description: '',
      address: '',
      maxHourlyCons: 0
    };
  }

  showToast(severity: string, summary: string): void {
    this.messageService.add({ severity, summary, life: 3000 });
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
