import { of, throwError } from 'rxjs';
import { CdkDragDrop } from '@angular/cdk/drag-drop';
import { TableroComponent } from './tablero.component';
import { Tarea } from '../tareas/tarea.models';

// Frontend counterpart to the backend's TareaService.MoverAsync tests: drop()
// applies the CDK reorder optimistically and, on a failed mover() call, must
// roll both columns (and the moved task's columnaId) back to their pre-drag state.
describe('TableroComponent.drop', () => {
    let component: TableroComponent;
    let tareaServiceSpy: jasmine.SpyObj<any>;
    let messageServiceSpy: jasmine.SpyObj<any>;

    function tarea(id: string, columnaId: string, orden: number): Tarea {
        return {
            id,
            titulo: `Tarea ${id}`,
            descripcion: '',
            prioridad: 'Media',
            fechaCreacion: '2026-01-01T00:00:00Z',
            orden,
            responsableId: null,
            responsableNombre: null,
            columnaId,
            proyectoId: 'p1'
        };
    }

    function containerRef(data: Tarea[]): any {
        return { data };
    }

    // previousContainer/container must be the SAME object reference for a
    // same-column drag: drop() detects "misma columna" via `===` identity,
    // not by comparing the underlying arrays.
    function dragEvent(previousContainer: any, container: any, previousIndex: number, currentIndex: number): CdkDragDrop<Tarea[]> {
        return {
            previousContainer,
            container,
            previousIndex,
            currentIndex,
            item: {} as any,
            isPointerOverContainer: true,
            distance: { x: 0, y: 0 },
            dropPoint: { x: 0, y: 0 },
            event: {} as any
        } as CdkDragDrop<Tarea[]>;
    }

    beforeEach(() => {
        tareaServiceSpy = jasmine.createSpyObj('TareaService', ['mover']);
        messageServiceSpy = jasmine.createSpyObj('MessageService', ['add']);

        component = new TableroComponent(
            {} as any,
            {} as any,
            {} as any,
            tareaServiceSpy,
            messageServiceSpy,
            {} as any
        );
        component.proyectoId = 'p1';
    });

    it('reorders within the same column and calls mover() with the full ordered id list', () => {
        const c1Tareas = [tarea('t1', 'c1', 0), tarea('t2', 'c1', 1), tarea('t3', 'c1', 2)];
        const columna = { id: 'c1', nombre: 'Todo', orden: 0, proyectoId: 'p1', tareas: c1Tareas } as any;
        component.columnas = [columna];

        tareaServiceSpy.mover.and.returnValue(of(tarea('t1', 'c1', 2)));

        // t1 (index 0) se mueve al final (index 2) dentro de la misma columna.
        const c1Container = containerRef(c1Tareas);
        const event = dragEvent(c1Container, c1Container, 0, 2);
        component.drop(event, columna);

        expect(columna.tareas.map((t: Tarea) => t.id)).toEqual(['t2', 't3', 't1']);
        expect(tareaServiceSpy.mover).toHaveBeenCalledWith('p1', 't1', {
            columnaDestinoId: 'c1',
            ordenIds: ['t2', 't3', 't1']
        });
    });

    it('rolls both columns and the columnaId back when the server rejects the move', () => {
        const origenTareas = [tarea('a', 'origen', 0), tarea('b', 'origen', 1)];
        const destinoTareas = [tarea('x', 'destino', 0)];
        const columnaOrigen = { id: 'origen', nombre: 'Todo', orden: 0, proyectoId: 'p1', tareas: origenTareas } as any;
        const columnaDestino = { id: 'destino', nombre: 'Hecho', orden: 1, proyectoId: 'p1', tareas: destinoTareas } as any;
        component.columnas = [columnaOrigen, columnaDestino];

        tareaServiceSpy.mover.and.returnValue(throwError(() => new Error('server rejected the move')));

        // 'a' (index 0 de origen) se arrastra al final de destino.
        const event = dragEvent(containerRef(origenTareas), containerRef(destinoTareas), 0, 1);
        component.drop(event, columnaDestino);

        expect(columnaOrigen.tareas.map((t: Tarea) => t.id)).toEqual(['a', 'b']);
        expect(columnaDestino.tareas.map((t: Tarea) => t.id)).toEqual(['x']);
        expect(columnaOrigen.tareas[0].columnaId).toBe('origen');
        expect(messageServiceSpy.add).toHaveBeenCalledWith(jasmine.objectContaining({ severity: 'error' }));
    });
});
