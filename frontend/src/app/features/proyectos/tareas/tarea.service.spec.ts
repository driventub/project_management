import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { environment } from 'src/environments/environment';
import { TareaService } from './tarea.service';
import { MoverTareaRequest } from './tarea.models';

describe('TareaService', () => {
    let service: TareaService;
    let httpMock: HttpTestingController;
    const proyectoId = 'proyecto-1';

    beforeEach(() => {
        TestBed.configureTestingModule({
            imports: [HttpClientTestingModule],
            providers: [TareaService]
        });
        service = TestBed.inject(TareaService);
        httpMock = TestBed.inject(HttpTestingController);
    });

    afterEach(() => httpMock.verify());

    it('getByProyecto() GETs the project-scoped tareas endpoint', () => {
        service.getByProyecto(proyectoId).subscribe();

        const req = httpMock.expectOne(`${environment.apiUrl}/proyectos/${proyectoId}/tareas`);
        expect(req.request.method).toBe('GET');
        req.flush([]);
    });

    it('mover() PUTs the destination column and the full ordered id list', () => {
        const request: MoverTareaRequest = { columnaDestinoId: 'col-2', ordenIds: ['t1', 't2', 't3'] };

        service.mover(proyectoId, 't2', request).subscribe();

        const req = httpMock.expectOne(`${environment.apiUrl}/proyectos/${proyectoId}/tareas/t2/mover`);
        expect(req.request.method).toBe('PUT');
        expect(req.request.body).toEqual(request);
        req.flush({});
    });
});
