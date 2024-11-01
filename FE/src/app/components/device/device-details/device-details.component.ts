import { ChangeDetectionStrategy, ChangeDetectorRef, Component, OnInit, ViewChild } from '@angular/core';
import { DeviceService } from '../../../services/Device/device.service';
import { DeviceShow } from '../../../interfaces/DeviceShow';
import { ActivatedRoute, NavigationStart } from '@angular/router';
import { JWTTokenService } from '../../../services/JWTToken/jwttoken.service';
import { UserRole } from '../../../enums/user-roles.enum';
import { Observable } from 'rxjs';
import { UtilityService } from '../../../services/Utility/utility.service';
import { MessageService } from 'primeng/api';

@Component({
  selector: 'device-details',
  templateUrl: './device-details.component.html',
  styleUrls: ['./device-details.component.scss']
})
export class DeviceDetailsComponent implements OnInit {
  deviceId: number | null = null;
  device: DeviceShow | null = null;
  imageUrl: string = 'assets/computer.png';
  details: string[] = [];
  showForm: boolean = false;
  isAdmin: boolean = false;

  constructor(
    private route: ActivatedRoute,
    private deviceService: DeviceService,
    private messageService: MessageService,
    private jwtService: JWTTokenService,
    private utilityService: UtilityService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      this.deviceId = +params['id'];
      this.loadDeviceDetails();
      this.checkIfAdmin();
      
    });
  }

  loadDeviceDetails(): void {
    if (this.deviceId !== null) {
      this.deviceService.getDeviceById(this.deviceId).subscribe(device => {
        this.device = device;
        //console.log(this.deviceId);
      }, error => {
        console.error('Error fetching device details:', error);
      });
    }
  }

  toggleForm(): void {
    this.showForm = !this.showForm;
  }

  deleteDevice(): void {
    if (this.deviceId) {
      console.log('Delete button clicked for device ID:', this.deviceId);
  
      this.deviceService.deleteDevice(this.deviceId).subscribe(
        () => {
          this.showToast('success', 'Device deleted successfully.');
        },
        (error) => {
          console.error('Error deleting device:', error);
          this.showToast('error', 'Failed to delete device.');
        }
      );
    }
  }
  

  updateDevice(formValue: any): void {
    if (this.deviceId !== null && this.device) {
      const updatedDevice: DeviceShow = {
        id: this.device.id,
        deviceName: formValue.DeviceName || this.device.deviceName,
        description: formValue.Description || this.device.description,
        address: formValue.Address || this.device.address,
        maxHourlyCons: formValue.MaxHourlyCons || this.device.maxHourlyCons,
      };

      this.deviceService.updateDevice(this.deviceId, updatedDevice).subscribe(
        (updatedDeviceResponse) => {
          this.device = updatedDeviceResponse;
          this.showToast('success', 'Device updated successfully.');
          this.toggleForm();
        },
        (error) => {
          console.error('Error updating device:', error);
          this.showToast('error', 'Failed to update device.');
        }
      );
    }
  }

  showToast(severity: string, summary: string): void {
    this.messageService.add({ severity, summary, life: 3000 });
  }

  checkIfAdmin(): void {
    const role = this.jwtService.getUserRole();
    this.isAdmin = this.utilityService.checkIfAdmin();
  }
}
