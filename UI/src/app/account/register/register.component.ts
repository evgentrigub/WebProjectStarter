import { Component } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import { Router } from '@angular/router';
import { first, switchMap } from 'rxjs/operators';
import { MatSnackBar } from '@angular/material';
import { AuthenticationService } from 'src/app/core/services/authentication.service';
import { UserDTO } from 'src/app/core/models/user';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css'],
})
export class RegisterComponent {
  readonly registerForm: FormGroup;
  readonly repeatPasswordControl: FormControl;

  hidePassword = true;
  isLoading = false;

  constructor(
    private formBuilder: FormBuilder,
    private router: Router,
    private authenticationService: AuthenticationService,
    private snackbar: MatSnackBar
  ) {
    if (this.authenticationService.currentUserValue) {
      this.router.navigate(['/']);
    }
    this.registerForm = this.formBuilder.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', Validators.required],
    });

    // TO-DO дописать валидатор на проверку повторного контрола
    this.repeatPasswordControl = this.formBuilder.control('', [Validators.required]);
  }

  /**
   * Check validation for registration
   */
  canSubmit(): boolean {
    return this.registerForm.valid;
  }

  /**
   * Sumbit the register form for registration
   */
  onSubmit(): void {
    if (!this.canSubmit()) {
      return;
    }

    this.isLoading = true;
    const user = this.registerForm.value as UserDTO;
    setTimeout(() => {
      this.authenticationService
        .register(user)
        .pipe(
          first(),
          switchMap(_ => {
            return this.authenticationService.login(user.email, user.password);
          })
        )
        .subscribe(
          _ => {
            this.router.navigate(['/home']);
            this.showMessage('С возвращением!');
            this.isLoading = false;
          },
          error => {
            this.isLoading = false;
            this.showMessage(error);
          }
        );
    }, 1500);
  }

  private showMessage(message: any) {
    this.snackbar.open(message, 'OK', { duration: 3000 });
  }
}
