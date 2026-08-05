import { Component, EventEmitter, Input, OnChanges, Output, SimpleChanges } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Columna, ColumnaRequest } from '../columna.models';
import { ColumnaService } from '../columna.service';

@Component({
    selector: 'app-columna-form',
    templateUrl: './columna-form.component.html'
})
export class ColumnaFormComponent implements OnChanges {

    @Input() visible = false;
    @Input() proyectoId!: string;
    @Input() columna: Columna | null = null;

    @Output() visibleChange = new EventEmitter<boolean>();
    @Output() saved = new EventEmitter<void>();

    form: FormGroup;
    saving = false;
    errorMessage: string | null = null;

    constructor(private fb: FormBuilder, private columnaService: ColumnaService) {
        this.form = this.fb.group({
            nombre: ['', [Validators.required, Validators.maxLength(200)]]
        });
    }

    ngOnChanges(changes: SimpleChanges): void {
        if (changes['visible'] && this.visible) {
            this.errorMessage = null;
            this.form.reset({ nombre: this.columna?.nombre ?? '' });
        }
    }

    get isEdit(): boolean {
        return !!this.columna;
    }

    submit(): void {
        if (this.form.invalid) {
            this.form.markAllAsTouched();
            return;
        }

        const request: ColumnaRequest = { nombre: this.form.value.nombre };

        this.saving = true;
        this.errorMessage = null;

        const request$ = this.isEdit
            ? this.columnaService.update(this.proyectoId, this.columna!.id, request)
            : this.columnaService.create(this.proyectoId, request);

        request$.subscribe({
            next: () => {
                this.saving = false;
                this.saved.emit();
                this.close();
            },
            error: (err) => {
                this.saving = false;
                this.errorMessage = err?.error?.message ?? 'No se pudo guardar la columna.';
            }
        });
    }

    close(): void {
        this.visible = false;
        this.visibleChange.emit(false);
    }
}
