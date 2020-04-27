import { Component, OnInit } from '@angular/core';
import { FormGroup, Validators, FormControl } from '@angular/forms';

import { AuthenticationService } from '../../helpers/authentication.service';
import { UserLogin } from '../shared/models/userLogin';
import { Router, ActivatedRoute } from '@angular/router';
import { first } from 'rxjs/operators';
import { CustomValidator } from 'src/helpers/customValidatior';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {
  public userLogin: UserLogin = new UserLogin();
  public loginForm = new FormGroup({
    'userName': new FormControl('', [
      Validators.required,
      Validators.minLength(5)
    ]),
    'password': new FormControl('', [
      Validators.required,
      Validators.minLength(8),
      CustomValidator.pattern(/^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[!@#\$%\^&\*])(?=.{8,})/ig)
    ]),
    'isRememberMe': new FormControl(false)
  });
  private returnUrl: string;
  public error = '';
  public isLoading = false;

  constructor(
    private authenticationService: AuthenticationService,
    private route: ActivatedRoute,
    private router: Router) {
    if (this.authenticationService.currentUserValue) {
      this.router.navigate(['/']);
    }
  }

  ngOnInit() {
    this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/';
  }

  get form() {
    return this.loginForm.controls;
  }

  public submit(): void {
    this.isLoading = true;
    this.authenticationService.login(this.loginForm.value).pipe(first())
      .subscribe(
        data => {
          this.router.navigate([this.returnUrl]);
        },
        error => {
          console.log(error);
          this.isLoading = false;
        });
  }
}
