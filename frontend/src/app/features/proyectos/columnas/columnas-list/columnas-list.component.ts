import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ConfirmationService, MessageService } from 'primeng/api';
import { Columna, ReorderColumnasRequest } from '../columna.models';
import { ColumnaService } from '../columna.service';

@Component({
    selector: 'app-columnas-list',
    templateUrl: './columnas-list.component.html',
    providers: [ConfirmationService, MessageService]
})
export class ColumnasListComponent implements OnInit {

    proyectoId!: string;
    columnas: Columna[] = [];
    loading = false;

    formVisible = false;
    columnaEnEdicion: Columna | null = null;

    constructor(
        private route: ActivatedRoute,
        private router: Router,
        private columnaService: ColumnaService,
        private confirmationService: ConfirmationService,
        private messageService: MessageService
    ) { }

    ngOnInit(): void {
        this.proyectoId = this.route.snapshot.paramMap.get('id')!;
        this.load();
    }

    load(): void {
        this.loading = true;
        this.columnaService.getByProyecto(this.proyectoId).subscribe({
            next: (columnas) => {
                this.columnas = columnas;
                this.loading = false;
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

    openCreate(): void {
        this.columnaEnEdicion = null;
        this.formVisible = true;
    }

    openEdit(columna: Columna): void {
        this.columnaEnEdicion = columna;
        this.formVisible = true;
    }

    onSaved(): void {
        this.messageService.add({ severity: 'success', summary: 'Listo', detail: 'Columna guardada.' });
        this.load();
    }

    confirmDelete(columna: Columna): void {
        this.confirmationService.confirm({
            message: `¿Eliminar la columna "${columna.nombre}"?`,
            header: 'Confirmar eliminación',
            icon: 'pi pi-exclamation-triangle',
            accept: () => this.delete(columna)
        });
    }

    private delete(columna: Columna): void {
        this.columnaService.delete(this.proyectoId, columna.id).subscribe({
            next: () => {
                this.messageService.add({ severity: 'success', summary: 'Listo', detail: 'Columna eliminada.' });
                this.load();
            },
            error: (err) => {
                const detail = err?.status === 409
                    ? 'No se puede eliminar una columna que contiene tareas.'
                    : 'No se pudo eliminar la columna.';
                this.messageService.add({ severity: 'error', summary: 'Error', detail });
            }
        });
    }

    moveUp(index: number): void {
        if (index === 0) {
            return;
        }
        this.swapAndReorder(index, index - 1);
    }

    moveDown(index: number): void {
        if (index === this.columnas.length - 1) {
            return;
        }
        this.swapAndReorder(index, index + 1);
    }

    private swapAndReorder(indexA: number, indexB: number): void {
        const reordered = [...this.columnas];
        [reordered[indexA], reordered[indexB]] = [reordered[indexB], reordered[indexA]];

        const request: ReorderColumnasRequest = {
            items: reordered.map((columna, index) => ({ id: columna.id, orden: index }))
        };

        this.columnaService.reorder(this.proyectoId, request).subscribe({
            next: (columnas) => {
                this.columnas = columnas;
            },
            error: () => {
                this.messageService.add({ severity: 'error', summary: 'Error', detail: 'No se pudo reordenar.' });
            }
        });
    }
}
