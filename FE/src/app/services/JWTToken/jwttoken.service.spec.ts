import { TestBed } from '@angular/core/testing';

import { JWTTokenService } from './jwttoken.service';

describe('JWTTokenServiceService', () => {
  let service: JWTTokenService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(JWTTokenService);
  });


});
