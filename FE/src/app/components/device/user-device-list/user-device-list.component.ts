import { Component, Input } from '@angular/core';
import { DeviceShow } from '../../../interfaces/DeviceShow';
import { DeviceService } from '../../../services/Device/device.service';
import { MessageService } from 'primeng/api';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-user-device-list',
  templateUrl: './user-device-list.component.html',
  styleUrls: ['./user-device-list.component.scss'],
})
export class UserDeviceListComponent {
  @Input() devices: DeviceShow[] = [];
  unownedDevices: DeviceShow[] = [];
  isEditDevices: boolean = false;
  userId: number | null = null;

  constructor(
    private deviceService: DeviceService,
    private messageService: MessageService,
    private route: ActivatedRoute,
  ) {}

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      this.userId = +params['id'];
    });
  }


  toggleEditDevices(): void {
    this.isEditDevices = !this.isEditDevices;
    if (this.isEditDevices) {
      this.fetchUnownedDevices();
    }
  }

  fetchUnownedDevices(): void {
    this.deviceService.getUnownedDevices().subscribe(
      (unownedDevices) => {
        this.unownedDevices = unownedDevices;
      },
      (error) => {
        this.showToast("error", "Failed to fetch unowned devices.");
      }
    );
  }

  assignDeviceToUser(deviceId: number): void {
    if (this.userId == null) {
      this.showToast("error", "User ID is missing.");
      return;
    }
    
    const personReference = { userId: this.userId! };
  
    this.deviceService.assignDeviceToUser(deviceId, personReference).subscribe(
      () => {
        this.showToast("success", "Device assigned successfully!");
        this.unownedDevices = this.unownedDevices.filter(device => device.id !== deviceId);
      },
      (error) => {
        this.showToast("error", "Failed to assign the device.");
      }
    );
  }
  

  removeDevice(deviceId: number): void {
    this.deviceService.removeDeviceFromUser(deviceId).subscribe(
      () => {
        this.devices = this.devices.filter(device => device.id !== deviceId);
        this.showToast("success", "Device removed successfully!");
      },
      error => {
        this.showToast("error", "Failed to remove the device.");
      }
    );
  }

  showToast(severity: string, summary: string) {
    this.messageService.add({ severity, summary, life: 3000 });
  }
}
