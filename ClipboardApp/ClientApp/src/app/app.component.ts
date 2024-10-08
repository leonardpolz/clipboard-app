import { Component } from '@angular/core';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { RouterModule } from '@angular/router';

@Component({
  standalone: true,
  imports: [
    RouterModule,
    NavMenuComponent,
  ],
  providers: [],
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  title = 'app';
}
