import { Component, Input, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';

import { NotificationService } from './notification.service';
import { Alert, AlertType } from '../shared/models/alert';
import { Router, NavigationStart } from '@angular/router';

@Component({
    selector: 'app-notification',
    templateUrl: './notification.component.html',
    styleUrls: ['./notification.component.css']
})
export class NotificationComponent implements OnInit, OnDestroy {
    @Input() id = 'default-alert';

    alerts: Alert[] = [];
    alertSubscription: Subscription;
    routeSubscription: Subscription;

    constructor(
        private router: Router,
        private alertService: NotificationService) { }

    ngOnInit() {
        this.alertSubscription = this.alertService.onAlert(this.id)
            .subscribe(alert => {
                this.alerts.push(alert);
            });

        this.routeSubscription = this.router.events.subscribe(event => {
            if (event instanceof NavigationStart) {
                this.alertService.clear(this.id);
            }
        });
    }

    ngOnDestroy() {
        this.alertSubscription.unsubscribe();
        this.routeSubscription.unsubscribe();
    }

    removeAlert(alert: Alert) {
        if (!this.alerts.includes(alert)) { return; }

        this.alerts = this.alerts.filter(x => x !== alert);
    }

    cssClass(alert: Alert) {
        if (!alert) { return; }

        const classes = ['alert', 'alert-dismissable'];

        const alertTypeClass = {
            [AlertType.Success]: 'alert alert-success',
            [AlertType.Error]: 'alert alert-danger',
            [AlertType.Info]: 'alert alert-info',
            [AlertType.Warning]: 'alert alert-warning'
        };

        classes.push(alertTypeClass[alert.type]);

        return classes.join(' ');
    }
}
