import { Component, EventEmitter, Input, OnChanges, Output, SimpleChanges } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Usuario } from 'src/app/core/usuarios/usuario.models';
import { UsuarioService } from 'src/app/core/usuarios/usuario.service';
import { PRIORIDADES, Tarea, TareaRequest } from '../tarea.models';
import { TareaService } from '../tarea.service';

@Component({
    selector: 'app-tarea-form',
    templateUrl: './tarea-form.component.html'
})
export class TareaFormComponent implements OnChanges {

    @Input() visible = false;
    @Input() proyectoId!: string;
    @Input() columnaId!: string;
    @Input() tarea: Tarea | null = null;

    @Output() visibleChange = new EventEmitter<boolean>();
    @Output() saved = new EventEmitter<void>();

    prioridades = PRIORIDADES;
    usuarios: Usuario[] = [];

    form: FormGroup;
    saving = false;
    errorMessage: string | null = null;

    constructor(private fb: FormBuilder, private tareaService: TareaService, private usuarioService: UsuarioService) {
        this.form = this.fb.group({
            titulo: ['', [Validators.required, Validators.maxLength(200)]],
            descripcion: ['', [Validators.maxLength(2000)]],
            prioridad: ['Media', Validators.required],
            responsableId: [null]
        });
    }

    ngOnChanges(changes: SimpleChanges): void {
        if (changes['visible'] && this.visible) {
            this.errorMessage = null;
            if (this.usuarios.length === 0) {
                this.usuarioService.getAll().subscribe(usuarios => this.usuarios = usuarios);
            }
            this.form.reset({
                titulo: this.tarea?.titulo ?? '',
                descripcion: this.tarea?.descripcion ?? '',
                prioridad: this.tarea?.prioridad ?? 'Media',
                responsableId: this.tarea?.responsableId ?? null
            });
        }
    }

    get isEdit(): boolean {
        return !!this.tarea;
    }

    submit(): void {
        if (this.form.invalid) {
            this.form.markAllAsTouched();
            return;
        }

        const request: TareaRequest = {
            columnaId: this.columnaId,
            titulo: this.form.value.titulo,
            descripcion: this.form.value.descripcion ?? '',
            prioridad: this.form.value.prioridad,
            responsableId: this.form.value.responsableId
        };

        this.saving = true;
        this.errorMessage = null;

        const request$ = this.isEdit
            ? this.tareaService.update(this.proyectoId, this.tarea!.id, request)
            : this.tareaService.create(this.proyectoId, request);

        request$.subscribe({
            next: () => {
                this.saving = false;
                this.saved.emit();
                this.close();
            },
            error: (err) => {
                this.saving = false;
                this.errorMessage = err?.error?.message ?? 'No se pudo guardar la tarea.';
            }
        });
    }

    close(): void {
        this.visible = false;
        this.visibleChange.emit(false);
    }
}
