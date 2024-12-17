import { Component, OnInit } from '@angular/core';
import { DeviceService } from '../../../services/Device/device.service';
import { Device } from '../../../interfaces/Device';
import { DeviceConsumption } from '../../../interfaces/DeviceConsumption';
import { JWTTokenService } from '../../../services/JWTToken/jwttoken.service';
import { UtilityService } from '../../../services/Utility/utility.service';
import { DatePipe } from '@angular/common';
import { ChartOptions, ChartData } from 'chart.js';
import { NgChartsModule } from 'ng2-charts';


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

  isAdmin: boolean = false;

  chartData: ChartData<'line'> = {
    labels: [],
    datasets: [
      {
        data: [],
        label: 'Consumption (kWh)',
        borderColor: 'rgba(0,123,255,1)',
        backgroundColor: 'rgba(0,123,255,0.2)',
        fill: true
      }
    ]
  };
  chartOptions: ChartOptions = {
    responsive: true,
    scales: {
      x: {
        title: {
          display: true,
          text: 'Hour of Day'
        }
      },
      y: {
        title: {
          display: true,
          text: 'Energy (kWh)'
        },
        beginAtZero: true
      }
    }
  };

  constructor(
    private deviceService: DeviceService,
    private jwtService: JWTTokenService,
    private utilityService: UtilityService,
    private datePipe: DatePipe
  ) {}

  ngOnInit(): void {
    this.userId = this.jwtService.getUserId() ? Number(this.jwtService.getUserId()) : null;
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
    if (this.userId) {
      this.deviceService.getDevicesByUserId(this.userId).subscribe(
        devices => {          this.devices = devices;
          if (devices.length > 0) {
            this.deviceId = devices[0].id;
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
  
    if (this.deviceId && this.selectedDate) {  
      this.deviceService.getDeviceConsumption(this.deviceId).subscribe({
        next: (data: DeviceConsumption) => {  
          if (data && data.hourlyConsumption) {  
            const selectedDateObj = new Date(this.selectedDate);
            const selectedYear = selectedDateObj.getUTCFullYear();
            const selectedMonth = selectedDateObj.getUTCMonth();
            const selectedDay = selectedDateObj.getUTCDate();
  
            this.hourlyConsumption = Object.entries(data.hourlyConsumption)
              .map(([timestamp, consumption]) => {
                const date = new Date(Number(timestamp));
                return {
                  timestamp: date,
                  consumption: consumption as number
                };
              })
              .filter(entry => {
                const year = entry.timestamp.getUTCFullYear();
                const month = entry.timestamp.getUTCMonth();
                const day = entry.timestamp.getUTCDate();
                return year === selectedYear && month === selectedMonth && day === selectedDay;
              })
              .map(entry => {
                const formattedTime = entry.timestamp.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
                return {
                  timestamp: formattedTime,
                  consumption: entry.consumption
                };
              });  
            if (this.hourlyConsumption.length > 0) {
              this.updateChartData();
              this.errorMessage = null;
            } else {
              this.errorMessage = 'No consumption data available for the selected day.';
            }
          } else {
            this.hourlyConsumption = [];
            this.errorMessage = 'No consumption data available for the selected day.';
          }
        },
        error: (err) => {
          this.errorMessage = err?.error?.message || 'An error occurred while fetching data.';
        }
      });
    } else {
      console.warn('Device ID or selected date is missing.');
    }
  }
  
  updateChartData(): void {
    const labels = this.hourlyConsumption.map(entry => entry.timestamp);
    const consumptionValues = this.hourlyConsumption.map(entry => entry.consumption);

    this.chartData = {
      labels: labels,
      datasets: [
        {
          data: consumptionValues,
          label: 'Consumption (kWh)',
          borderColor: 'rgba(0,123,255,1)',
          backgroundColor: 'rgba(0,123,255,0.2)',
          fill: true
        }
      ]
    };
  }

  onDateChange(): void {
    console.log("Date selected:", this.selectedDate);
    this.loadDeviceConsumption();
    this.formattedDate = this.datePipe.transform(this.selectedDate, 'mediumDate') || '';
    console.log("Formatted date:", this.formattedDate);
  }

}
