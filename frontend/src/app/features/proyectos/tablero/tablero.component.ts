import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CdkDragDrop, moveItemInArray, transferArrayItem } from '@angular/cdk/drag-drop';
import { MessageService } from 'primeng/api';
import { Columna } from '../columnas/columna.models';
import { ColumnaService } from '../columnas/columna.service';
import { Tarea } from '../tareas/tarea.models';
import { TareaService } from '../tareas/tarea.service';
import { TableroRealtimeService } from './tablero-realtime.service';
import { TareaMovidaNotification } from './tablero-realtime.models';

interface ColumnaConTareas extends Columna {
    tareas: Tarea[];
}

@Component({
    selector: 'app-tablero',
    templateUrl: './tablero.component.html',
    styleUrls: ['./tablero.component.scss'],
    providers: [MessageService]
})
export class TableroComponent implements OnInit, OnDestroy {

    proyectoId!: string;
    columnas: ColumnaConTareas[] = [];
    loading = false;

    formVisible = false;
    tareaEnEdicion: Tarea | null = null;
    columnaDestinoCreacion: string | null = null;

    constructor(
        private route: ActivatedRoute,
        private router: Router,
        private columnaService: ColumnaService,
        private tareaService: TareaService,
        private messageService: MessageService,
        private tableroRealtime: TableroRealtimeService
    ) { }

    ngOnInit(): void {
        this.proyectoId = this.route.snapshot.paramMap.get('id')!;
        this.load();
        this.conectarRealtime();
    }

    ngOnDestroy(): void {
        this.tableroRealtime.desconectar();
    }

    get columnaIds(): string[] {
        return this.columnas.map(c => c.id);
    }

    load(): void {
        this.loading = true;
        this.columnaService.getByProyecto(this.proyectoId).subscribe({
            next: (columnas) => {
                this.tareaService.getByProyecto(this.proyectoId).subscribe({
                    next: (tareas) => {
                        this.columnas = columnas
                            .slice()
                            .sort((a, b) => a.orden - b.orden)
                            .map(columna => ({
                                ...columna,
                                tareas: tareas
                                    .filter(t => t.columnaId === columna.id)
                                    .sort((a, b) => a.orden - b.orden)
                            }));
                        this.loading = false;
                    },
                    error: () => {
                        this.loading = false;
                        this.messageService.add({ severity: 'error', summary: 'Error', detail: 'No se pudieron cargar las tareas.' });
                    }
                });
            },
            error: () => {
                this.loading = false;
                this.messageService.add({ severity: 'error', summary: 'Error', detail: 'No se pudieron cargar las columnas.' });
            }
        });
    }

    goBack(): void {
        this.router.navigate(['/']);
    }

    openCreate(columna: ColumnaConTareas): void {
        this.tareaEnEdicion = null;
        this.columnaDestinoCreacion = columna.id;
        this.formVisible = true;
    }

    openEdit(tarea: Tarea): void {
        this.tareaEnEdicion = tarea;
        this.columnaDestinoCreacion = tarea.columnaId;
        this.formVisible = true;
    }

    onSaved(): void {
        this.messageService.add({ severity: 'success', summary: 'Listo', detail: 'Tarea guardada.' });
        this.load();
    }

    deleteTarea(tarea: Tarea): void {
        this.tareaService.delete(this.proyectoId, tarea.id).subscribe({
            next: () => {
                this.messageService.add({ severity: 'success', summary: 'Listo', detail: 'Tarea eliminada.' });
                this.load();
            },
            error: () => {
                this.messageService.add({ severity: 'error', summary: 'Error', detail: 'No se pudo eliminar la tarea.' });
            }
        });
    }

    prioridadSeverity(prioridad: string): string {
        switch (prioridad) {
            case 'Urgente': return 'danger';
            case 'Alta': return 'warning';
            case 'Media': return 'info';
            default: return 'success';
        }
    }

    drop(event: CdkDragDrop<Tarea[]>, columnaDestino: ColumnaConTareas): void {
        const origenSnapshot = [...event.previousContainer.data];
        const mismaColumna = event.previousContainer === event.container;
        const destinoSnapshot = mismaColumna ? origenSnapshot : [...event.container.data];
        const tareaMovida = event.previousContainer.data[event.previousIndex];
        const columnaOrigenId = tareaMovida.columnaId;

        if (mismaColumna) {
            moveItemInArray(event.container.data, event.previousIndex, event.currentIndex);
        } else {
            transferArrayItem(event.previousContainer.data, event.container.data, event.previousIndex, event.currentIndex);
            tareaMovida.columnaId = columnaDestino.id;
        }

        const ordenIds = event.container.data.map(t => t.id);

        this.tareaService.mover(this.proyectoId, tareaMovida.id, { columnaDestinoId: columnaDestino.id, ordenIds }).subscribe({
            error: () => {
                event.previousContainer.data.splice(0, event.previousContainer.data.length, ...origenSnapshot);
                if (!mismaColumna) {
                    event.container.data.splice(0, event.container.data.length, ...destinoSnapshot);
                }
                tareaMovida.columnaId = columnaOrigenId;
                this.messageService.add({ severity: 'error', summary: 'Error', detail: 'No se pudo mover la tarea.' });
            }
        });
    }

    private conectarRealtime(): void {
        this.tableroRealtime.conectar(this.proyectoId);
        this.tableroRealtime.onTareaCreada(tarea => this.aplicarTareaCreada(tarea));
        this.tableroRealtime.onTareaActualizada(tarea => this.aplicarTareaActualizada(tarea));
        this.tableroRealtime.onTareaEliminada(tareaId => this.aplicarTareaEliminada(tareaId));
        this.tableroRealtime.onTareaMovida(notificacion => this.aplicarTareaMovida(notificacion));

        this.tableroRealtime.iniciar().catch(() => {
            this.messageService.add({ severity: 'warn', summary: 'Tiempo real', detail: 'No se pudo conectar al canal en tiempo real.' });
        });
    }

    // Estas tres se aplican también al autor del cambio (recibe su propio evento):
    // el estado local ya se refrescó vía load()/drop() antes de que llegara el evento,
    // así que el merge por id abajo es idempotente y no rompe nada.
    private aplicarTareaCreada(tarea: Tarea): void {
        const columna = this.columnas.find(c => c.id === tarea.columnaId);
        if (columna && !columna.tareas.some(t => t.id === tarea.id)) {
            columna.tareas = [...columna.tareas, tarea].sort((a, b) => a.orden - b.orden);
        }
    }

    private aplicarTareaActualizada(tarea: Tarea): void {
        // La edición nunca cambia columnaId (eso solo lo hace mover), así que
        // basta con localizar su columna actual y reemplazarla ahí.
        const columna = this.columnas.find(c => c.id === tarea.columnaId);
        if (!columna) {
            return;
        }
        const index = columna.tareas.findIndex(t => t.id === tarea.id);
        columna.tareas = index === -1
            ? [...columna.tareas, tarea].sort((a, b) => a.orden - b.orden)
            : columna.tareas.map((t, i) => i === index ? tarea : t);
    }

    private aplicarTareaEliminada(tareaId: string): void {
        for (const columna of this.columnas) {
            columna.tareas = columna.tareas.filter(t => t.id !== tareaId);
        }
    }

    private aplicarTareaMovida(notificacion: TareaMovidaNotification): void {
        const origen = this.columnas.find(c => c.id === notificacion.columnaOrigenId);
        const destino = this.columnas.find(c => c.id === notificacion.columnaDestinoId);

        if (origen) {
            origen.tareas = [...notificacion.tareasColumnaOrigen].sort((a, b) => a.orden - b.orden);
        }
        if (destino && destino !== origen) {
            destino.tareas = [...notificacion.tareasColumnaDestino].sort((a, b) => a.orden - b.orden);
        }
    }
}
