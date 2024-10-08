import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatTabsModule } from "@angular/material/tabs";
import { NavigationEnd, Router, RouterEvent, RouterModule } from "@angular/router";
import { filter, Subscription } from "rxjs";
import { AuthService } from '../shared/services/auth-service/auth.service';
import { ClipboardModule } from '@angular/cdk/clipboard';
import { MatIconModule } from '@angular/material/icon';

@Component({
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatTabsModule,
    RouterModule,
    ClipboardModule,
    MatIconModule
  ],
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit, OnDestroy {
  links = [
    {
      label: 'Text',
      route: './text',
    },
    {
      label: 'Files',
      route: './files',
    }
  ];

  sessionUrl: string = "";
  copied: boolean = false;

  activeLink: string = null!;
  private routerSubscription: Subscription = null!;

  constructor(private _router: Router, private _authService: AuthService) { }

  ngOnInit(): void {
    const lastSegment = this._router.url.split('/').pop();
    if (lastSegment === 'files') {
      this.activeLink = this.links[1].route;
    } else {
      this.activeLink = this.links[0].route;
    }

    this.routerSubscription = this._router.events.pipe(
      filter((event: any) => event instanceof NavigationEnd)
    ).subscribe((event: NavigationEnd) => {
      const lastSegment = this._router.url.split('/').pop();
      if (lastSegment === 'files') {
        this.activeLink = this.links[1].route;
      } else {
        this.activeLink = this.links[0].route;
      }
    });

    this.sessionUrl = `${location.origin}/${this._authService.getSessionId()}`;
  }

  copyToClipboard() {
    this.copied = true;
    navigator.clipboard.writeText(this.sessionUrl).then(
      () => {
        // Icon will change for 1500ms then revert back
        setTimeout(() => this.copied = false, 3000);
      },
      (err) => {
        console.error('Failed to copy content: ', err);
        this.copied = false;
      }
    );
  }

  ngOnDestroy(): void {
    this.routerSubscription.unsubscribe();
  }
}
