import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent } from '@angular/common/http';
import { Observable } from 'rxjs';

import { GetToken } from './auth.helper';

@Injectable()
export class JwtInterceptor implements HttpInterceptor {

    intercept(
        req: HttpRequest<any>,
        next: HttpHandler): Observable<HttpEvent<any>> {

        const idToken = GetToken('Authorization');

        if (idToken) {
            const cloned = req.clone({
                headers: req.headers.set('Authorization',
                    `Bearer ${idToken}`)
            });

            return next.handle(cloned);
        } else {
            return next.handle(req);
        }
    }
}
