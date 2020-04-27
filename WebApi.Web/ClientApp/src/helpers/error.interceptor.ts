import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError, from } from 'rxjs';
import { catchError } from 'rxjs/operators';

import { AuthenticationService } from './authentication.service';
import { NotificationService } from 'src/app/notification/notification.service';
import { SaveToken, GetToken } from './auth.helper';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
    constructor(
        private authenticationService: AuthenticationService,
        private notificationService: NotificationService) { }

    public intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        return next.handle(request).pipe(catchError(error => {

            if (error instanceof HttpErrorResponse && error.status === 401) {

                if (error.headers.has('Token-Expired')) {
                    return from(this.handle401Error(request, next));

                } else {
                    this.authenticationService.logout();
                }

            } else {
                const errorMessage = error.error.Message || error.statusText;
                this.notificationService.error(errorMessage.error.Message);
                return throwError(errorMessage);
            }
        }));
    }

    private async addToken(request: HttpRequest<any>, token: any) {
        return request.clone({
            setHeaders: {
                'Authorization': `Bearer ${GetToken('Authorization')}`
            }
        });
    }

    private async handle401Error(request: HttpRequest<any>, next: HttpHandler) {
        const token = from(await this.authenticationService.refreshToken()).subscribe(result => {
            SaveToken('Authorization', result.token);
            SaveToken('RefreshToken', result.refreshToken);
        });

        const newRequest = await this.addToken(request, token);

        return next.handle(newRequest).toPromise();
    }
}
