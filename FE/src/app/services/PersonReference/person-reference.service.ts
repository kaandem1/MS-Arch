import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Device } from '../../interfaces/Device';
import { DeviceCreate } from '../../interfaces/DeviceCreate';
import { DeviceUpdate } from '../../interfaces/DeviceUpdate';
import { PersonReference } from '../../interfaces/PersonReference';
import { deviceBaseUrl } from '../../constants/api-constants';
import { deviceEndpoint } from '../../constants/api-constants';

@Injectable({
  providedIn: 'root'
})
export class PersonReferenceService {
  private url = deviceBaseUrl + deviceEndpoint;
  constructor(private http: HttpClient) { }

  createPersonReference(personReferenceDTO: PersonReference): Observable<PersonReference> {
    return this.http.post<PersonReference>(`${this.url}`, personReferenceDTO);
  }

  deletePersonReference(userId: number): Observable<void> {
    return this.http.delete<void>(`${this.url}/${userId}`);
  }

  getAllPersonReferences(): Observable<PersonReference[]> {
    return this.http.get<PersonReference[]>(`${this.url}`);
  }
}
