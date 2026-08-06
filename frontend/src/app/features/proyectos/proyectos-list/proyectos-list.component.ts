import { Component } from '@angular/core';
import { HttpResponse } from '@angular/common/http';
import { TableLazyLoadEvent } from 'primeng/table';
import { ConfirmationService, MessageService } from 'primeng/api';
import { Subject, debounceTime } from 'rxjs';
import { Proyecto } from '../proyecto.models';
import { ProyectoService } from '../proyecto.service';
import { FormatoReporte, ReporteService } from '../reportes/reporte.service';

@Component({
    selector: 'app-proyectos-list',
    templateUrl: './proyectos-list.component.html',
    providers: [ConfirmationService, MessageService]
})
export class ProyectosListComponent {

    proyectos: Proyecto[] = [];
    totalRecords = 0;
    loading = false;
    pageSize = 10;
    nombreFilter = '';

    formVisible = false;
    proyectoEnEdicion: Proyecto | null = null;

    private lastEvent: TableLazyLoadEvent = { first: 0, rows: this.pageSize };
    private filterChanged = new Subject<void>();

    constructor(
        private proyectoService: ProyectoService,
        private reporteService: ReporteService,
        private confirmationService: ConfirmationService,
        private messageService: MessageService
    ) {
        this.filterChanged.pipe(debounceTime(300)).subscribe(() => {
            this.lastEvent = { ...this.lastEvent, first: 0 };
            this.load(this.lastEvent);
        });
    }

    load(event: TableLazyLoadEvent): void {
        this.lastEvent = event;
        const pageSize = event.rows ?? this.pageSize;
        const pageNumber = Math.floor((event.first ?? 0) / pageSize) + 1;

        this.loading = true;
        this.proyectoService.getPaged(pageNumber, pageSize, this.nombreFilter).subscribe({
            next: (result) => {
                this.proyectos = result.items;
                this.totalRecords = result.totalCount;
                this.loading = false;
            },
            error: () => {
                this.loading = false;
                this.messageService.add({ severity: 'error', summary: 'Error', detail: 'No se pudieron cargar los proyectos.' });
            }
        });
    }

    onFilterChange(): void {
        this.filterChanged.next();
    }

    openCreate(): void {
        this.proyectoEnEdicion = null;
        this.formVisible = true;
    }

    openEdit(proyecto: Proyecto): void {
        this.proyectoEnEdicion = proyecto;
        this.formVisible = true;
    }

    onSaved(): void {
        this.messageService.add({ severity: 'success', summary: 'Listo', detail: 'Proyecto guardado.' });
        this.load(this.lastEvent);
    }

    confirmDelete(proyecto: Proyecto): void {
        this.confirmationService.confirm({
            message: `¿Eliminar el proyecto "${proyecto.nombre}"?`,
            header: 'Confirmar eliminación',
            icon: 'pi pi-exclamation-triangle',
            accept: () => this.delete(proyecto)
        });
    }

    exportarReporte(proyecto: Proyecto, formato: FormatoReporte): void {
        this.reporteService.exportar(proyecto.id, formato).subscribe({
            next: (response) => this.descargarBlob(response),
            error: () => {
                this.messageService.add({ severity: 'error', summary: 'Error', detail: 'No se pudo generar el reporte.' });
            }
        });
    }

    private descargarBlob(response: HttpResponse<Blob>): void {
        if (!response.body) {
            return;
        }

        const nombreArchivo = this.extraerNombreArchivo(response.headers.get('Content-Disposition')) ?? 'reporte';
        const url = window.URL.createObjectURL(response.body);
        const link = document.createElement('a');
        link.href = url;
        link.download = nombreArchivo;
        link.click();
        window.URL.revokeObjectURL(url);
    }

    private extraerNombreArchivo(contentDisposition: string | null): string | null {
        if (!contentDisposition) {
            return null;
        }

        const match = /filename="?([^"]+)"?/.exec(contentDisposition);
        return match ? match[1] : null;
    }

    private delete(proyecto: Proyecto): void {
        this.proyectoService.delete(proyecto.id).subscribe({
            next: () => {
                this.messageService.add({ severity: 'success', summary: 'Listo', detail: 'Proyecto eliminado.' });
                this.load(this.lastEvent);
            },
            error: () => {
                this.messageService.add({ severity: 'error', summary: 'Error', detail: 'No se pudo eliminar el proyecto.' });
            }
        });
    }
}
