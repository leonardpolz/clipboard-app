import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { saveAs } from 'file-saver';
import { MatButtonModule } from "@angular/material/button";
import { MatSnackBar, MatSnackBarModule } from "@angular/material/snack-bar";
import { BinaryFileClientCustom } from '../shared/services/upload-file.client';
import { AuthService } from '../shared/services/auth-service/auth.service';
import { WsFileNameClient } from '../shared/services/fileName-ws-client';

@Component({
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatButtonModule,
    MatSnackBarModule
  ],
  selector: 'app-files',
  templateUrl: './files.component.html',
  styleUrls: ['./files.component.scss']
})
export class FilesComponent implements OnInit, OnDestroy {
  currentFileName: string = "-";

  constructor(
    private _binaryFileClient: BinaryFileClientCustom,
    private _snackBar: MatSnackBar,
    private _authService: AuthService,
    private _wsFileNameClient: WsFileNameClient
  ) { }

  ngOnInit(): void {
    this.loadTextAsyc();
  }

  ngOnDestroy(): void {
    this._wsFileNameClient.disconnect();
  }

  private loadTextAsyc() {
    this._wsFileNameClient.connect(this._authService.getAuthToken() ?? "").subscribe({
      next: (message: string) => {
        this.currentFileName = message;
      },
      error: (error: any) => {
        console.error('WebSocket error:', error);
      }
    });
  }

  public downloadFile() {
    this._binaryFileClient.getBinaryFile().subscribe(response => {
      if (response.status === 200 && response.data) {
        const blob = new Blob([response.data], { type: 'application/octet-stream' });
        const fileName = response.headers!['content-disposition'].split(';')[1].split('=')[1].replace(/"/g, '');
        console.log(fileName)
        saveAs(blob, fileName);
        this.openSnackBar('Successfully download file!', 'dismiss');
      } else {
        this.openSnackBar('Failed to download file!', 'dismiss');
      }
    });
  }

  public onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;

    if (input.files && input.files.length > 0) {
      const file = input.files[0];
      this.uploadFile(file);
    }
  }

  public onDrop(event: DragEvent) {
    event.preventDefault();

    if (event.dataTransfer) {
      const files = event.dataTransfer.files;

      if (files.length > 0) {
        const file = files[0];

        this.uploadFile(file);
      }
    }
  }

  public onDragOver(event: DragEvent) {
    event.preventDefault();
  }

  public uploadFile(file: File) {
    let headers = [{
      'ContentType': [file.type],
    }];

    this._binaryFileClient.patchBinaryFile(file, file.type, null, headers, file.size, file.name, file.name).subscribe(response => {
      if (response.status === 200 || response.status === 206) {
        this.openSnackBar(`Successfully uploaded "${file.name}"`, 'dismiss')
      } else {
        this.openSnackBar(`Failed to upload "${file.name}"`, 'dismiss')
      }
    });
  }

  private openSnackBar(message: string, action: string) {
    this._snackBar.open(message, action, {
      duration: 2000,
    });
  }
}
