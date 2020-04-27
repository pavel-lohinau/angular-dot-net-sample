import { Component } from '@angular/core';

import { RegisterService } from './register.service';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { UserRegister } from '../shared/models/userRegister';
import { CustomValidator } from 'src/helpers/customValidatior';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css'],
  providers: [RegisterService]
})
export class RegisterComponent {
  public isLoading = false;
  public registerForm = new FormGroup({
    'firstName': new FormControl('', [
      Validators.required,
      Validators.minLength(1)
    ]),
    'lastName': new FormControl('', [
      Validators.required,
      Validators.minLength(1)
    ]),
    'userName': new FormControl('', [
      Validators.required,
      Validators.minLength(5)
    ]),
    'email': new FormControl('', [
      Validators.required,
      CustomValidator.pattern(/^[a-z0-9_]+(\.[_a-z0-9]+)*@[a-z0-9-]+(\.[a-z0-9-]+)*(\.[a-z]{2,15})$/ig)
    ]),
    'password': new FormControl('', [
      Validators.required,
      Validators.minLength(8),
      CustomValidator.pattern(/^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[!@#\$%\^&\*])(?=.{8,})/ig)
    ])
  });

  constructor(
    private registerService: RegisterService,
    private route: Router) { }

  get form() {
    return this.registerForm.controls;
  }

  public submit(): void {
    this.isLoading = true;
    this.registerService.register(this.registerForm.value).subscribe(res => {
      this.route.navigate(['/login']);
    },
      error => {
        console.log(error);
        this.isLoading = false;
      });
  }
}
