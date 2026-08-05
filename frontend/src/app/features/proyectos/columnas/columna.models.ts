export interface Columna {
    id: string;
    nombre: string;
    orden: number;
    proyectoId: string;
}

export interface ColumnaRequest {
    nombre: string;
}

export interface ReorderColumnaItem {
    id: string;
    orden: number;
}

export interface ReorderColumnasRequest {
    items: ReorderColumnaItem[];
}
