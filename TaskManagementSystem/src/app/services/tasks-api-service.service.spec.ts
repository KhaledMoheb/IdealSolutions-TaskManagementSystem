import { TestBed } from '@angular/core/testing';

import { TasksApiServiceService } from './tasks-api-service.service';

describe('TasksApiServiceService', () => {
  let service: TasksApiServiceService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(TasksApiServiceService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
