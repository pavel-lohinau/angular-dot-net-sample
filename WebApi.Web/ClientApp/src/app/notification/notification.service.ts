import { Injectable } from '@angular/core';
import { Observable, Subject } from 'rxjs';
import { Alert, AlertType } from '../shared/models/alert';
import { filter } from 'rxjs/operators';

@Injectable({ providedIn: 'root' })
export class NotificationService {
    private subject = new Subject<Alert>();
    private defaultId = 'default-alert';

    onAlert(id = this.defaultId): Observable<Alert> {
        return this.subject.asObservable().pipe(filter(x => x && x.id === id));
    }

    success(message: string) {
        this.alert(new Alert({ type: AlertType.Success, message }));
    }

    error(message: string) {
        this.alert(new Alert({ type: AlertType.Error, message }));
    }

    info(message: string) {
        this.alert(new Alert({ type: AlertType.Info, message }));
    }

    warn(message: string) {
        this.alert(new Alert({ type: AlertType.Warning, message }));
    }

    alert(alert: Alert) {
        alert.id = alert.id || this.defaultId;
        this.subject.next(alert);
    }

    clear(id = this.defaultId) {
        this.subject.next(null);
    }
}
