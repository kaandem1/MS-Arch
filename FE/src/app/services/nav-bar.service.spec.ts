import { TestBed } from '@angular/core/testing';

import { NavbarService } from './nav-bar.service';

describe('NavBarService', () => {
  let service: NavbarService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(NavbarService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
