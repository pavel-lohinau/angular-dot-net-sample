import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { User } from '../shared/models/user';

@Injectable()
export class RegisterService {

    constructor(private http: HttpClient) { }

    register(requestModel: User) {
        return this.http.post<any>('/api/account/register', requestModel);
    }
}
