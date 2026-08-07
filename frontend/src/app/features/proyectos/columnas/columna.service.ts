import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { Columna, ColumnaRequest, ReorderColumnasRequest } from './columna.models';

@Injectable({ providedIn: 'root' })
export class ColumnaService {

    constructor(private http: HttpClient) { }

    private baseUrl(proyectoId: string): string {
        return `${environment.apiUrl}/proyectos/${proyectoId}/columnas`;
    }

    getByProyecto(proyectoId: string): Observable<Columna[]> {
        return this.http.get<Columna[]>(this.baseUrl(proyectoId));
    }

    create(proyectoId: string, request: ColumnaRequest): Observable<Columna> {
        return this.http.post<Columna>(this.baseUrl(proyectoId), request);
    }

    update(proyectoId: string, id: string, request: ColumnaRequest): Observable<Columna> {
        return this.http.put<Columna>(`${this.baseUrl(proyectoId)}/${id}`, request);
    }

    delete(proyectoId: string, id: string): Observable<void> {
        return this.http.delete<void>(`${this.baseUrl(proyectoId)}/${id}`);
    }

    reorder(proyectoId: string, request: ReorderColumnasRequest): Observable<Columna[]> {
        return this.http.put<Columna[]>(`${this.baseUrl(proyectoId)}/reorder`, request);
    }
}
