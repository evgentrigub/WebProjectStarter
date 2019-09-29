import { Component, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { UserViewModel } from '../core/models/user';
import { Subscription } from 'rxjs';
import { AuthenticationService } from '../core/services/authentication.service';

@Component({
  selector: 'app-nav-bar',
  templateUrl: './nav-bar.component.html',
  styleUrls: ['./nav-bar.component.css'],
})
export class NavBarComponent implements OnDestroy {
  isShow: boolean;
  currentUser: UserViewModel;
  currentUserSubscription: Subscription;

  constructor(private router: Router, private authenticationService: AuthenticationService) {
    this.currentUserSubscription = this.authenticationService.currentUser.subscribe(user => {
      if (user) {
        this.isShow = true;
      } else {
        this.isShow = false;
      }
      this.currentUser = user;
    });
  }

  ngOnDestroy(): void {
    this.currentUserSubscription.unsubscribe();
  }

  logout() {
    this.authenticationService.logout();
    this.router.navigate(['/login']);
  }
}
