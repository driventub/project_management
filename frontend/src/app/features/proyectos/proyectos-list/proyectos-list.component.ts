import { Component } from '@angular/core';
import { TableLazyLoadEvent } from 'primeng/table';
import { ConfirmationService, MessageService } from 'primeng/api';
import { Subject, debounceTime } from 'rxjs';
import { Proyecto } from '../proyecto.models';
import { ProyectoService } from '../proyecto.service';

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
