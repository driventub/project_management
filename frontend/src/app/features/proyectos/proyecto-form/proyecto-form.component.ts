import { Component, EventEmitter, Input, OnChanges, Output, SimpleChanges } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ESTADOS_PROYECTO, EstadoProyecto, Proyecto, ProyectoRequest } from '../proyecto.models';
import { ProyectoService } from '../proyecto.service';

@Component({
    selector: 'app-proyecto-form',
    templateUrl: './proyecto-form.component.html'
})
export class ProyectoFormComponent implements OnChanges {

    @Input() visible = false;
    @Input() proyecto: Proyecto | null = null;

    @Output() visibleChange = new EventEmitter<boolean>();
    @Output() saved = new EventEmitter<void>();

    form: FormGroup;
    saving = false;
    errorMessage: string | null = null;
    estados: EstadoProyecto[] = ESTADOS_PROYECTO;

    constructor(private fb: FormBuilder, private proyectoService: ProyectoService) {
        this.form = this.fb.group({
            nombre: ['', [Validators.required, Validators.maxLength(200)]],
            descripcion: ['', [Validators.maxLength(1000)]],
            fechaInicio: [null as Date | null, Validators.required],
            fechaFinEsperada: [null as Date | null, Validators.required],
            estado: ['Planificado', Validators.required]
        });
    }

    ngOnChanges(changes: SimpleChanges): void {
        if (changes['visible'] && this.visible) {
            this.errorMessage = null;
            if (this.proyecto) {
                this.form.reset({
                    nombre: this.proyecto.nombre,
                    descripcion: this.proyecto.descripcion,
                    fechaInicio: this.parseDate(this.proyecto.fechaInicio),
                    fechaFinEsperada: this.parseDate(this.proyecto.fechaFinEsperada),
                    estado: this.proyecto.estado
                });
            } else {
                this.form.reset({
                    nombre: '',
                    descripcion: '',
                    fechaInicio: null,
                    fechaFinEsperada: null,
                    estado: 'Planificado'
                });
            }
        }
    }

    get isEdit(): boolean {
        return !!this.proyecto;
    }

    submit(): void {
        if (this.form.invalid) {
            this.form.markAllAsTouched();
            return;
        }

        const value = this.form.value;
        const request: ProyectoRequest = {
            nombre: value.nombre,
            descripcion: value.descripcion ?? '',
            fechaInicio: this.formatDate(value.fechaInicio),
            fechaFinEsperada: this.formatDate(value.fechaFinEsperada),
            estado: value.estado
        };

        this.saving = true;
        this.errorMessage = null;

        const request$ = this.isEdit
            ? this.proyectoService.update(this.proyecto!.id, request)
            : this.proyectoService.create(request);

        request$.subscribe({
            next: () => {
                this.saving = false;
                this.saved.emit();
                this.close();
            },
            error: (err) => {
                this.saving = false;
                this.errorMessage = err?.error?.message ?? 'No se pudo guardar el proyecto.';
            }
        });
    }

    close(): void {
        this.visible = false;
        this.visibleChange.emit(false);
    }

    private formatDate(date: Date): string {
        const year = date.getFullYear();
        const month = String(date.getMonth() + 1).padStart(2, '0');
        const day = String(date.getDate()).padStart(2, '0');
        return `${year}-${month}-${day}`;
    }

    private parseDate(value: string): Date {
        const [year, month, day] = value.split('-').map(Number);
        return new Date(year, month - 1, day);
    }
}
