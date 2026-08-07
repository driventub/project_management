import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { MoverTareaRequest, Tarea, TareaRequest } from './tarea.models';

@Injectable({ providedIn: 'root' })
export class TareaService {

    constructor(private http: HttpClient) { }

    private baseUrl(proyectoId: string): string {
        return `${environment.apiUrl}/proyectos/${proyectoId}/tareas`;
    }

    getByProyecto(proyectoId: string): Observable<Tarea[]> {
        return this.http.get<Tarea[]>(this.baseUrl(proyectoId));
    }

    create(proyectoId: string, request: TareaRequest): Observable<Tarea> {
        return this.http.post<Tarea>(this.baseUrl(proyectoId), request);
    }

    update(proyectoId: string, id: string, request: TareaRequest): Observable<Tarea> {
        return this.http.put<Tarea>(`${this.baseUrl(proyectoId)}/${id}`, request);
    }

    delete(proyectoId: string, id: string): Observable<void> {
        return this.http.delete<void>(`${this.baseUrl(proyectoId)}/${id}`);
    }

    mover(proyectoId: string, id: string, request: MoverTareaRequest): Observable<Tarea> {
        return this.http.put<Tarea>(`${this.baseUrl(proyectoId)}/${id}/mover`, request);
    }
}
