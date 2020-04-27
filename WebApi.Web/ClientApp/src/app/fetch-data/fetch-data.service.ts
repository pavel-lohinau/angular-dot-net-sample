import { HttpClient } from '@angular/common/http';
import { Injectable, Inject } from '@angular/core';

import { Customer } from '../shared/models/customer';

@Injectable()
export class FetchDataService {
    private customers: Array<Customer>;

    public constructor(private httpClient: HttpClient) { }

    public getData(): Array<Customer> {
        this.httpClient.get<Array<Customer>>('/api/customers').subscribe(result => {
            this.customers = result;
        }, error => console.error(error));

        return this.customers;
    }
}
