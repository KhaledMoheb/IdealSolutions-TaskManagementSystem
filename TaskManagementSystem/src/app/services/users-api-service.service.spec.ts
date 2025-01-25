import { TestBed } from '@angular/core/testing';

import { UsersApiServiceService } from './users-api-service.service';

describe('UsersApiServiceService', () => {
  let service: UsersApiServiceService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(UsersApiServiceService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
