import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../shared/services/auth-service/auth.service';
import { ActivatedRoute, Router } from '@angular/router';
import {MatFormFieldModule} from "@angular/material/form-field";
import {MatInputModule} from "@angular/material/input";
import {MatButtonModule} from "@angular/material/button";
import {CommonModule} from "@angular/common";
import {MatSnackBar, MatSnackBarModule} from "@angular/material/snack-bar";
import {MatCardModule} from "@angular/material/card";

@Component({
  selector: 'app-auth',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatSnackBarModule,
    MatCardModule
  ],
  templateUrl: './auth.component.html',
  styleUrls: ['./auth.component.scss']
})
export class AuthComponent implements OnInit{
  sessionKey: string = "";

  constructor(private _authService: AuthService, private _router: Router, private _snackBar: MatSnackBar, private _activatedRoute: ActivatedRoute) {}

  ngOnInit(): void {
    if (this._authService.hasUserValidToken()) {
      this._router.navigate(["home"]);
    }

    this._activatedRoute.url.subscribe(urlSegments => {
      if (urlSegments[urlSegments.length - 1].path !== "auth") {
        let sessionId = urlSegments[urlSegments.length - 1].path;
        this._authService.authenticateGuest(sessionId)
          .then((response) => {
            this._router.navigateByUrl('/', { skipLocationChange: true }).then(() => {
              this._router.navigate(['/home']);
            });
          })
          .catch((error) => {
            console.error(error);
            this.openSnackBar("Login Failed", "Dismiss");
          });
      }
    });
  }

  public guestLogin() {
    this._authService.authenticateGuest()
      .then((response) => {
        this._router.navigate(["home"]);
      })
      .catch((error) => {
        console.error(error);
        this.openSnackBar("Login Failed", "Dismiss");
      });
  }

  public joinSession(sessionKey: string) {
    this._router.navigate([`/${sessionKey}`]);
  }

  private openSnackBar(message: string, action: string) {
    this._snackBar.open(message, action, {
      duration: 2000,
    });
  }
}
