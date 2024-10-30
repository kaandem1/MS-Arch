import { TestBed } from '@angular/core/testing';

import { PersonReferenceService } from './person-reference.service';

describe('PersonReferenceService', () => {
  let service: PersonReferenceService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(PersonReferenceService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
