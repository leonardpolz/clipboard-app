import { Injectable } from "@angular/core";
import { environment } from "src/environments/environment";
import jwtDecode from "jwt-decode";
import { AuthGuestClient } from "../backend.client";

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  authTokenName = "auth-token"
  _injector: any

  constructor(
    private _authGuestClient: AuthGuestClient,
  ) { }

  public authenticateGuest(sessionId: string | null = null): Promise<any> {
    return new Promise((resolve, reject) => {
      this._authGuestClient.authenticate(sessionId).subscribe({
        next: result => {
          const reader = new FileReader();
          reader.onload = () => {
            try {
              const jsonData = JSON.parse(reader.result as string);
              localStorage.setItem(this.authTokenName, jsonData.authToken);
              resolve(jsonData);
            } catch (error) {
              reject('Error processing response: ' + error);
            }
          };
          reader.onerror = () => {
            reject('Error reading response data');
          };
          reader.readAsText(result.data);
        },
        error: error => {
          reject(error);
        }
      });
    });
  }


  public hasUserValidToken(): boolean {

    let authToken = localStorage.getItem(this.authTokenName)

    if (!authToken)
      return false;

    let tokenObject: { aud: string, iss: string, exp: number, sub: string } = jwtDecode(authToken ? authToken : "")

    if (tokenObject.exp <= Math.floor((new Date).getTime() / 1000))
      return false;

    if (tokenObject.aud != environment.apiRoot)
      return false;

    if (tokenObject.iss != environment.apiRoot)
      return false;

    return true;
  }

  public getSessionId(): string {

    let authToken = localStorage.getItem(this.authTokenName);

    let tokenObject: { sessionId: string } = jwtDecode(authToken ? authToken : "");

    return tokenObject.sessionId;
  }

  public loggoutUser() {
    localStorage.clear();
  }

  public getAuthToken() {
    return localStorage.getItem(this.authTokenName);
  }
}
