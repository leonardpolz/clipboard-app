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
  sessionKey: string = "";
  copiedUrl: boolean = false;
  copiedKey: boolean = false;

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

    this.sessionKey = this._authService.getSessionId();
    this.sessionUrl = `${location.origin}/${this._authService.getSessionId()}`;
  }

  copyToClipboardUrl() {

    this.copiedUrl = true;
    navigator.clipboard.writeText(this.sessionUrl).then(
      () => {
        setTimeout(() => this.copiedUrl = false, 3000);
      },
      (err) => {
        console.error('Failed to copy content: ', err);
        this.copiedUrl = false;
      }
    );
  }

  copyToClipboardKey() {

    this.copiedKey = true;
    navigator.clipboard.writeText(this.sessionKey).then(
      () => {
        setTimeout(() => this.copiedKey = false, 3000);
      },
      (err) => {
        console.error('Failed to copy content: ', err);
        this.copiedKey = false;
      }
    );
  }

  ngOnDestroy(): void {
    this.routerSubscription.unsubscribe();
  }
}
