import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Device } from '../../../interfaces/Device';
import { DeviceService } from '../../../services/Device/device.service';
import { Router } from '@angular/router';
import { UtilityService } from '../../../services/Utility/utility.service';


@Component({
  selector: 'my-devices',
  templateUrl: './my-devices.component.html',
  styleUrls: ['./my-devices.component.scss']
})
export class MyDevicesComponent implements OnInit {
  userId: number | null = null;
  devices: Device[] = [];
  filteredDevices: Device[] = [];
  maxHourlyConsumptionFilter: number | null = null;
  error: string | null = null;
  isAdmin: boolean = false;

  constructor(
    private route: ActivatedRoute,
    private deviceService: DeviceService,
    private utilityService: UtilityService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.isAdmin = this.utilityService.checkIfAdmin();
    if(this.isAdmin){
      this.router.navigate(['/home']);
    }
    this.route.paramMap.subscribe(params => {
      const idParam = params.get('id');
      this.userId = idParam ? +idParam : null;
      if (this.userId) {
        this.getDevicesForUser(this.userId);
      } else {
        this.error = "User ID is not available.";
      }
    });
  }

  getDevicesForUser(userId: number): void {
    this.deviceService.getDevicesByUserId(userId).subscribe(
      devices => {
        this.devices = devices;
        this.filteredDevices = devices;
      },
      error => {
        this.error = "Error fetching devices: " + error;
      }
    );
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
