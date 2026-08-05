export type Prioridad = 'Baja' | 'Media' | 'Alta' | 'Urgente';

export const PRIORIDADES: Prioridad[] = ['Baja', 'Media', 'Alta', 'Urgente'];

export interface Tarea {
    id: string;
    titulo: string;
    descripcion: string;
    prioridad: Prioridad;
    fechaCreacion: string;
    orden: number;
    responsableId: string | null;
    responsableNombre: string | null;
    columnaId: string;
    proyectoId: string;
}

export interface TareaRequest {
    columnaId: string;
    titulo: string;
    descripcion: string;
    prioridad: Prioridad;
    responsableId: string | null;
}

export interface MoverTareaRequest {
    columnaDestinoId: string;
    ordenIds: string[];
}
