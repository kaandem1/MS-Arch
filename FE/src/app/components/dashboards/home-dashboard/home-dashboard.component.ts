import { Component, OnInit } from '@angular/core';
import { DeviceService } from '../../../services/Device/device.service';
import { Device } from '../../../interfaces/Device';
import { DeviceConsumption } from '../../../interfaces/DeviceConsumption';
import { JWTTokenService } from '../../../services/JWTToken/jwttoken.service';
import { UtilityService } from '../../../services/Utility/utility.service';
import { DatePipe } from '@angular/common'; 

@Component({
  selector: 'home-dashboard',
  templateUrl: './home-dashboard.component.html',
  styleUrls: ['./home-dashboard.component.scss'],
  providers: [DatePipe]
})
export class HomeDashboardComponent implements OnInit {
  userId: number | null = null;
  devices: Device[] = [];
  deviceId: number = 1;
  selectedDate: string = new Date().toISOString().split('T')[0];
  consumptionData: DeviceConsumption | null = null;
  errorMessage: string | null = null;

  formattedDate: string = '';
  hourlyConsumption: { timestamp: string, consumption: number }[] = [];

  isAdmin: boolean = true;

  constructor(
    private deviceService: DeviceService,
    private jwtService: JWTTokenService,
    private utilityService: UtilityService,
    private datePipe: DatePipe
  ) {}

  ngOnInit(): void {
    console.log("HomeDashboardComponent initialized.");
    this.userId = this.jwtService.getUserId() ? Number(this.jwtService.getUserId()) : null;
    console.log("User ID fetched from JWT:", this.userId);

    if (this.userId) {
      this.loadUserDevices();
    } else {
      this.errorMessage = 'User ID is not available.';
      console.error(this.errorMessage);
    }

    this.checkIfAdmin();
  }

  checkIfAdmin(): void {
    const role = this.jwtService.getUserRole();
    console.log("User role:", role);
    this.isAdmin = this.utilityService.checkIfAdmin();
    console.log("Is Admin:", this.isAdmin);
  }

  loadUserDevices(): void {
    console.log("Loading devices for user ID:", this.userId);
    if (this.userId) {
      this.deviceService.getDevicesByUserId(this.userId).subscribe(
        devices => {
          console.log("Devices fetched:", devices);
          this.devices = devices;
          if (devices.length > 0) {
            this.deviceId = devices[0].id;
            console.log("First device selected (ID):", this.deviceId);
            this.loadDeviceConsumption();
          } else {
            this.errorMessage = 'No devices found for the logged-in user.';
            console.error(this.errorMessage);
          }
        },
        error => {
          this.errorMessage = 'Error fetching devices: ' + error;
          console.error(this.errorMessage);
        }
      );
    }
  }

  loadDeviceConsumption(): void {
    console.log("Loading consumption for device ID:", this.deviceId, "on date:", this.selectedDate);
  
    if (this.deviceId && this.selectedDate) {
      console.log(`Fetching consumption data for Device ID: ${this.deviceId}`);
      
      this.deviceService.getDeviceConsumption(this.deviceId).subscribe({
        next: (data: DeviceConsumption) => {
          console.log("Device consumption data received:", data);
  
          if (data && data.hourlyConsumption) {
            console.log("Hourly consumption data:", data.hourlyConsumption);
  
            this.consumptionData = data;
            this.hourlyConsumption = Object.entries(data.hourlyConsumption).map(([timestamp, consumption]) => {
              const formattedTimestamp = new Date(parseInt(timestamp)).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
              console.log(`Processed entry - Timestamp: ${formattedTimestamp}, Consumption: ${consumption}`);
              return { 
                timestamp: formattedTimestamp, 
                consumption 
              };
            });
          } else {
            console.warn("No hourly consumption data available.");
          }
        },
        error: (err) => {
          console.error('Error fetching device consumption:', err);
          console.log("Error details:", err);
          this.errorMessage = err?.error?.message || 'An error occurred while fetching data.';
        }
      });
    } else {
      console.warn('Device ID or selected date is missing.');
    }
  }
  
  

  onDateChange(): void {
    console.log("Date selected:", this.selectedDate);
    this.loadDeviceConsumption();
    this.formattedDate = this.datePipe.transform(this.selectedDate, 'mediumDate') || '';
    console.log("Formatted date:", this.formattedDate);
  }
}
