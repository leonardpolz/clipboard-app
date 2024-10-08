import {CommonModule} from '@angular/common';
import {Component, OnDestroy, OnInit} from '@angular/core';
import {FormsModule} from '@angular/forms';
import {MatButtonModule} from "@angular/material/button";
import {MatSnackBar, MatSnackBarModule} from "@angular/material/snack-bar";
import {AuthService} from '../shared/services/auth-service/auth.service';
import {WsFileNameClient} from '../shared/services/fileName-ws-client';
import {BinaryFileClient} from "../shared/services/backend.client";
import {MatProgressBar} from "@angular/material/progress-bar";

@Component({
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatButtonModule,
    MatSnackBarModule,
    MatProgressBar
  ],
  selector: 'app-files',
  templateUrl: './files.component.html',
  styleUrls: ['./files.component.scss']
})
export class FilesComponent implements OnInit, OnDestroy {
  currentFileName: string = "-";
  isLoading: boolean = false;

  constructor(
    private _binaryFileClient: BinaryFileClient,
    private _snackBar: MatSnackBar,
    private _authService: AuthService,
    private _wsFileNameClient: WsFileNameClient
  ) {
  }

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

  private downloadBlob(sasUri: string, fileName: string) {
    fetch(sasUri)
      .then(response => response.blob())
      .then(blob => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = fileName;
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
        window.URL.revokeObjectURL(url);
      })
      .catch(error => {
        console.error('Download failed:', error);
        this.openSnackBar('Failed to download file!', 'dismiss');
        this.isLoading = false;
      });
  }

  public downloadFile() {

    this._binaryFileClient.getBlobDownloadContext().subscribe(
      response => {
        this.isLoading = true;
        this.downloadBlob(response.sasUri!, response.originalFileName!);
        this.isLoading = false;
      },
      error => {
        this.openSnackBar('Failed to download file!', 'dismiss');
        this.isLoading = false;
      }
    )
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

  private uploadBlob(sasUri: string, file: File) {
    fetch(sasUri, {
      method: 'PUT',
      headers: {
        'x-ms-blob-type': 'BlockBlob',
        'Content-Type': file.type,
        'x-ms-meta-name': file.name
      },
      body: file
    })
      .then(response => {
        if (!response.ok) {
          this.openSnackBar('Failed to upload file to storage account!', 'dismiss');
          this.isLoading = false;
        }
      })
      .catch(error => {
        this.openSnackBar('Failed to upload file to storage account!', 'dismiss');
        this.isLoading = false;
      });
  }

  public uploadFile(file: File) {

    const decodedFileName = encodeURI(file.name);

    this._binaryFileClient.getBlobUploadContext(decodedFileName).subscribe(response => {
        this.isLoading = true;
        this.uploadBlob(response.sasUri!, file);
        this.openSnackBar(`Successfully uploaded "${file.name}"`, 'dismiss');
        this.isLoading = false;
      },
      error => {
        this.openSnackBar('Failed to upload file!', 'dismiss');
        this.isLoading = false;
      }
    )
  }

  private openSnackBar(message: string, action: string) {
    this._snackBar.open(message, action, {
      duration: 2000,
    });
  }
}
