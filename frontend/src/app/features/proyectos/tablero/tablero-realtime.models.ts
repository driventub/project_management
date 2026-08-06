import { Tarea } from '../tareas/tarea.models';

export interface TareaMovidaNotification {
    proyectoId: string;
    columnaOrigenId: string;
    columnaDestinoId: string;
    tareasColumnaOrigen: Tarea[];
    tareasColumnaDestino: Tarea[];
}
