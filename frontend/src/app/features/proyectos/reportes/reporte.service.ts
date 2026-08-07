import { Injectable } from '@angular/core';
import { HttpClient, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';

export type FormatoReporte = 'pdf' | 'xlsx';

@Injectable({ providedIn: 'root' })
export class ReporteService {

    constructor(private http: HttpClient) { }

    exportar(proyectoId: string, formato: FormatoReporte): Observable<HttpResponse<Blob>> {
        return this.http.get(`${environment.apiUrl}/proyectos/${proyectoId}/reportes/${formato}`, {
            observe: 'response',
            responseType: 'blob'
        });
    }
}
