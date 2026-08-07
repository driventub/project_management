import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { environment } from 'src/environments/environment';
import { AuthService } from 'src/app/core/auth/auth.service';
import { Columna } from '../columnas/columna.models';
import { Tarea } from '../tareas/tarea.models';
import { TareaMovidaNotification } from './tablero-realtime.models';

@Injectable({ providedIn: 'root' })
export class TableroRealtimeService {

    private connection: signalR.HubConnection | null = null;
    private proyectoId: string | null = null;

    constructor(private authService: AuthService) { }

    conectar(proyectoId: string): void {
        this.proyectoId = proyectoId;
        this.connection = new signalR.HubConnectionBuilder()
            .withUrl(environment.hubUrl, {
                accessTokenFactory: () => this.authService.getToken() ?? '',
                withCredentials: false
            })
            .withAutomaticReconnect()
            .build();

        this.connection.onreconnected(() => this.unirseATablero());
    }

    async iniciar(): Promise<void> {
        if (!this.connection) {
            return;
        }
        await this.connection.start();
        await this.unirseATablero();
    }

    onTareaCreada(callback: (tarea: Tarea) => void): void {
        this.connection?.on('TareaCreada', callback);
    }

    onTareaActualizada(callback: (tarea: Tarea) => void): void {
        this.connection?.on('TareaActualizada', callback);
    }

    onTareaEliminada(callback: (tareaId: string) => void): void {
        this.connection?.on('TareaEliminada', callback);
    }

    onTareaMovida(callback: (notificacion: TareaMovidaNotification) => void): void {
        this.connection?.on('TareaMovida', callback);
    }

    onColumnaCreada(callback: (columna: Columna) => void): void {
        this.connection?.on('ColumnaCreada', callback);
    }

    onColumnaEliminada(callback: (columnaId: string) => void): void {
        this.connection?.on('ColumnaEliminada', callback);
    }

    async desconectar(): Promise<void> {
        if (!this.connection) {
            return;
        }

        if (this.proyectoId) {
            try {
                await this.connection.invoke('SalirTablero', this.proyectoId);
            } catch {
                // la conexión ya pudo haberse cerrado (ej. token expirado); no hay nada que limpiar
            }
        }

        await this.connection.stop();
        this.connection = null;
        this.proyectoId = null;
    }

    private async unirseATablero(): Promise<void> {
        if (this.connection && this.proyectoId) {
            await this.connection.invoke('UnirseTablero', this.proyectoId);
        }
    }
}
