import { Component, OnDestroy } from '@angular/core';
import { AuthenticationService } from '../../helpers/authentication.service';
import { User } from '../shared/models/user';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css']
})
export class NavMenuComponent {
  isExpanded = false;
  public currentUser: User;

  constructor(private authenticationService: AuthenticationService) {
    this.authenticationService.currentUser.subscribe(user => {
      this.currentUser = user;
    });
  }

  public collapse(): void {
    this.isExpanded = false;
  }

  public toggle(): void {
    this.isExpanded = !this.isExpanded;
  }

  public logout(): void {
    this.authenticationService.logout();
  }
}
