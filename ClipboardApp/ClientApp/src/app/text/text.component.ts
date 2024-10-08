import { CommonModule } from '@angular/common';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatTabsModule } from "@angular/material/tabs";
import { MatFormFieldModule } from "@angular/material/form-field";
import { MatInputModule } from "@angular/material/input";
import { MatButtonModule } from "@angular/material/button";
import { MatSnackBar, MatSnackBarModule } from "@angular/material/snack-bar";
import { WsTextClient } from '../shared/services/text-ws.client';
import { AuthService } from '../shared/services/auth-service/auth.service';
import {SetTextClipboardHandlerRequestDto, TextClient} from "../shared/services/backend.client";
import {MatIcon} from "@angular/material/icon";
import {MatProgressSpinner} from "@angular/material/progress-spinner";

@Component({
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatTabsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatSnackBarModule,
    MatIcon,
    MatProgressSpinner
  ],
  selector: 'app-text',
  templateUrl: './text.component.html',
  styleUrls: ['./text.component.scss']
})
export class TextComponent implements OnInit, OnDestroy {

  text: string = "";
  isWebSocketConnected: boolean = false

  constructor(
    private _textClient: TextClient,
    private _snackBar: MatSnackBar,
    private _wsTextClient: WsTextClient,
    private _authService: AuthService
  ) { }

  ngOnInit(): void {
    this.loadTextAsyc();
  }

  private loadTextAsyc() {
    this._wsTextClient.connect(this._authService.getAuthToken() ?? "").subscribe({
      next: (message: string) => {
        this.text = message;
        this.isWebSocketConnected = true;
      },
      error: (error: any) => {
        console.error('WebSocket error:', error);
      }
    });
  }

  public ngOnDestroy(): void {
    this._wsTextClient.disconnect();
  }

  public onTextChange(): void {
    this._wsTextClient.sendText(this.text);
  }

  public loadText() {
    this._textClient.getTextClipboard().subscribe(result => {
      if (result.text != null) {
        this.text = result.text;
      }
    });
  }

  public saveText() {
    let dto: SetTextClipboardHandlerRequestDto = { text: this.text };
    this._textClient.patchTextClipboard(dto).subscribe();
    this.openSnackBar('Successfully saved!', 'dismiss')
  }

  public clearText() {
    this.text = "";
    this.saveText();
    this.openSnackBar('Successfully cleared!', 'dismiss')
  }

  public copyText() {
    navigator.clipboard.writeText(this.text);
    this.openSnackBar('Copied to clipboard!', 'dismiss')
  }

  private openSnackBar(message: string, action: string) {
    this._snackBar.open(message, action, {
      duration: 2000,
    });
  }
}
