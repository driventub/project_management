import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { Usuario } from './usuario.models';

@Injectable({ providedIn: 'root' })
export class UsuarioService {

    constructor(private http: HttpClient) { }

    getAll(): Observable<Usuario[]> {
        return this.http.get<Usuario[]>(`${environment.apiUrl}/usuarios`);
    }
}
