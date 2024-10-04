import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthService } from '../../services/auth-service/auth.service';

@Injectable()
export class TokenInterceptorInterceptor implements HttpInterceptor {

  constructor(private _authService: AuthService) { }

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    
    let authToken = this._authService.getAuthToken();

    if(!authToken)
     return next.handle(req) 

    let tokenizedRequest = req.clone({
      setHeaders: {
        Authorization: `Bearer ${authToken}`                   
      }
    })
    return next.handle(tokenizedRequest)
  }
}
