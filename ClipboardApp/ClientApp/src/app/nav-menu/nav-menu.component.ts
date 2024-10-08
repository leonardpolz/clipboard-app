import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../shared/services/auth-service/auth.service';
import {MatToolbarModule} from "@angular/material/toolbar";
import {MatButtonModule} from "@angular/material/button";

@Component({
  standalone: true,
  imports: [
    RouterModule,
    CommonModule,
    MatToolbarModule,
    MatButtonModule,
  ],
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.scss']
})
export class NavMenuComponent {
  isExpanded = false;

  constructor(private _authService: AuthService, private _router: Router) {}

  collapse() {
    this.isExpanded = false;
  }

  toggle() {
    this.isExpanded = !this.isExpanded;
  }

  loginUser() {
    this._router.navigate(["auth"]);
  }

  logoutUser() {
    this._authService.loggoutUser();
    this._router.navigate(["auth"]);
  }

  isLoggedIn() {
    return this._authService.hasUserValidToken();
  }
}
