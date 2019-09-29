import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { first } from 'rxjs/operators';
import { MatSnackBar } from '@angular/material';
import { AuthenticationService } from 'src/app/core/services/authentication.service';

interface AuthModel {
  email: string;
  password: string;
}

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
})
export class LoginComponent {
  readonly loginForm: FormGroup;
  isLoading = false;
  hidePassword = true;

  constructor(
    private formBuilder: FormBuilder,
    private router: Router,
    private authenticationService: AuthenticationService,
    private snackbar: MatSnackBar
  ) {
    if (this.authenticationService.currentUserValue) {
      this.router.navigate(['/home']);
    }
    this.loginForm = this.formBuilder.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', Validators.required],
    });
  }

  /**
   * Check validation for authentication
   */
  canSubmit(): boolean {
    return this.loginForm.valid;
  }

  /**
   * Sumbit the login form for authentication
   */
  onSubmit(): void {
    if (!this.canSubmit()) {
      return;
    }

    if (this.loginForm.invalid) {
      return;
    }
    const user = this.loginForm.value as AuthModel;

    this.isLoading = true;
    this.authenticationService
      .login(user.email, user.password)
      .pipe(first())
      .subscribe(
        _ => {
          this.isLoading = false;
          this.router.navigate(['/home']);
          this.showMessage('Log in success!');
        },
        error => {
          this.isLoading = false;
          this.showMessage(error);
        }
      );
  }

  private showMessage(message: any) {
    this.snackbar.open(message, 'OK', { duration: 5000 });
  }
}
