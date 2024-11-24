import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { DeviceService } from '../../../services/Device/device.service';
import { Device } from '../../../interfaces/Device';

@Component({
  selector: 'search-results',
  templateUrl: './search-results.component.html',
  styleUrls: ['./search-results.component.scss']
})
export class SearchResultsComponent implements OnInit {
  searchTerm: string = '';
  devices: Device[] = [];
  isLoading: boolean = true;
  showUsers: boolean = false;
  showDevices: boolean = false;
 
  constructor(
    private route: ActivatedRoute,
    private deviceService : DeviceService
  ) {}
 
  ngOnInit(): void {
    this.route.params.subscribe(params => {
      this.searchTerm = params['name'];
      this.fetchAllData();
    });
  }
 
  fetchAllData(): void {
    this.isLoading = true;

    const handleResponse = (response: any[] | undefined) => response ?? [];

    const devices$ = this.deviceService.searchDevice(this.searchTerm).toPromise();

    Promise.allSettled([devices$])
      .then(results => {
        this.devices = handleResponse(results[0].status === 'fulfilled' ? results[0].value as Device[] : undefined);
        
        this.showDevices = this.devices.length > 0;
      })
      .catch(error => {
      })
      .finally(() => {
        this.isLoading = false;
      });
  }
  

  toggleFilter(type: string): void {
    if (type === 'devices') {
      this.showDevices = !this.showDevices;
    }
  }
}
