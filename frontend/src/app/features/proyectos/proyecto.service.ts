import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { PagedResult, Proyecto, ProyectoRequest } from './proyecto.models';

@Injectable({ providedIn: 'root' })
export class ProyectoService {

    private readonly baseUrl = `${environment.apiUrl}/proyectos`;

    constructor(private http: HttpClient) { }

    getPaged(pageNumber: number, pageSize: number, nombre?: string): Observable<PagedResult<Proyecto>> {
        let params = new HttpParams()
            .set('pageNumber', pageNumber)
            .set('pageSize', pageSize);

        if (nombre) {
            params = params.set('nombre', nombre);
        }

        return this.http.get<PagedResult<Proyecto>>(this.baseUrl, { params });
    }

    create(request: ProyectoRequest): Observable<Proyecto> {
        return this.http.post<Proyecto>(this.baseUrl, request);
    }

    update(id: string, request: ProyectoRequest): Observable<Proyecto> {
        return this.http.put<Proyecto>(`${this.baseUrl}/${id}`, request);
    }

    delete(id: string): Observable<void> {
        return this.http.delete<void>(`${this.baseUrl}/${id}`);
    }
}
