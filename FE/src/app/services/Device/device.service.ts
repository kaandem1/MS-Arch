import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Device } from '../../interfaces/Device';
import { DeviceCreate } from '../../interfaces/DeviceCreate';
import { DeviceUpdate } from '../../interfaces/DeviceUpdate';
import { PersonReference } from '../../interfaces/PersonReference';
import { deviceBaseUrl } from '../../constants/api-constants';
import { deviceEndpoint } from '../../constants/api-constants';
import { DeviceConsumption } from '../../interfaces/DeviceConsumption';
import { deviceCons } from '../../constants/api-constants';

@Injectable({
  providedIn: 'root'
})
export class DeviceService {

  private url = deviceBaseUrl + deviceEndpoint;
  private apiUrl = deviceBaseUrl + deviceCons;

  constructor(private http: HttpClient) { }

  getAllDevices(): Observable<Device[]> {
    return this.http.get<Device[]>(`${this.url}`);
  }

  getDeviceById(id: number): Observable<Device> {
    return this.http.get<Device>(`${this.url}/${id}`);
  }

  createDevice(device: DeviceCreate): Observable<Device> {
    return this.http.post<Device>(`${this.url}`, device);
  }

  updateDevice(id: number, device: DeviceUpdate): Observable<Device> {
    return this.http.put<Device>(`${this.url}/${id}`, device);
  }

  deleteDevice(id: number): Observable<void> {
    return this.http.delete<void>(`${this.url}/${id}`);
  }

  searchDevice(name: string): Observable<Device[]> {
    return this.http.get<Device[]>(`${this.url}/search?name=${name}`);
  }

  assignDeviceToUser(deviceId: number, personReference: PersonReference): Observable<void> {
    return this.http.put<void>(`${this.url}/${deviceId}/assign`, personReference);
  }

  removeDeviceFromUser(deviceId: number): Observable<void> {
    return this.http.patch<void>(`${this.url}/${deviceId}`, {});
  }

  clearDevicesForUser(userId: number): Observable<void> {
    return this.http.patch<void>(`${this.url}/clear/${userId}`, {});
  }

  getDevicesByUserId(userId: number): Observable<Device[]> {
    return this.http.get<Device[]>(`${this.url}/user/${userId}`);
  }

  getUnownedDevices(): Observable<Device[]> {
    return this.http.get<Device[]>(`${this.url}/unowned`);
  }

  getDeviceConsumption(deviceId: number): Observable<DeviceConsumption> {
    return this.http.get<DeviceConsumption>(`${this.apiUrl}/${deviceId}`);
  }
  
}
