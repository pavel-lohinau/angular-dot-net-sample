import { Component, OnInit } from '@angular/core';

import { FetchDataService } from './fetch-data.service';
import { Customer } from '../shared/models/customer';

@Component({
  selector: 'app-fetch-data',
  templateUrl: './fetch-data.component.html',
  providers: [FetchDataService]
})
export class FetchDataComponent implements OnInit {
  public customers: Array<Customer>;

  constructor(private fetchDataService: FetchDataService) { }

  ngOnInit(): void {
    this.customers = this.fetchDataService.getData();
  }
}
