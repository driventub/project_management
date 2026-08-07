export type EstadoProyecto = 'Planificado' | 'EnProgreso' | 'Completado' | 'Cancelado';

export const ESTADOS_PROYECTO: EstadoProyecto[] = ['Planificado', 'EnProgreso', 'Completado', 'Cancelado'];

export interface Proyecto {
    id: string;
    nombre: string;
    descripcion: string;
    fechaInicio: string;
    fechaFinEsperada: string;
    estado: EstadoProyecto;
}

export interface ProyectoRequest {
    nombre: string;
    descripcion: string;
    fechaInicio: string;
    fechaFinEsperada: string;
    estado: EstadoProyecto;
}

export interface PagedResult<T> {
    items: T[];
    totalCount: number;
    pageNumber: number;
    pageSize: number;
}
