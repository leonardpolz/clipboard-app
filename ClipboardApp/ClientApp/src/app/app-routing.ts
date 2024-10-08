import { Routes } from "@angular/router";
import { HomeComponent } from "./home/home.component";
import { AuthComponent } from "./auth/auth.component";
import { AuthGuard } from "./shared/guards/auth-guard/auth.guard";
import {TextComponent} from "./text/text.component";
import {FilesComponent} from "./files/files.component";

export const routes: Routes = [
    {
        path: '',
        redirectTo: 'home',
        pathMatch: 'full'
    },
    {
        path: 'home',
        component: HomeComponent,
        canActivate: [AuthGuard],
        children: [
          {
            path: '',
            pathMatch: 'prefix',
            redirectTo: 'text'
          },
          {
            path: 'text',
            component: TextComponent
          },
          {
            path: 'files',
            component: FilesComponent
          }
        ]
    },
    {
        path: ':sessionId',
        component: AuthComponent
    },
    {
      path: 'auth',
      component: AuthComponent
    }
];
