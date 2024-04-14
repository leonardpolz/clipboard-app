import { Inject, Injectable, Optional } from "@angular/core";
import { Observable } from "rxjs";
import { API_BASE_URL } from "./backend.client";

@Injectable()
export class WsTextClient {
    private webSocket: WebSocket | undefined;
    private baseUrl: string;

    constructor(@Optional() @Inject(API_BASE_URL) baseUrl?: string) {
        this.baseUrl = baseUrl ?? "";
    }

    connect(token: string): Observable<string> {
        return new Observable(observer => {
            const wsUrl = new URL(this.baseUrl.replace(/^http/, 'ws') + "/ws/text");
            wsUrl.searchParams.append('token', token);

            this.webSocket = new WebSocket(wsUrl.toString());

            this.webSocket.onmessage = (event) => {
                observer.next(event.data as string);
            };

            this.webSocket.onerror = (event) => {
                observer.error(event);
            };

            this.webSocket.onclose = () => {
                observer.complete();
            };

            return () => {
                this.disconnect();
            };
        });
    }

    disconnect(): void {
        if (this.webSocket) {
            this.webSocket.close();
            this.webSocket = undefined;
        }
    }

    sendText(text: string): void {
        if (this.webSocket && this.webSocket.readyState === WebSocket.OPEN) {
            this.webSocket.send(text);
        }
    }
}
