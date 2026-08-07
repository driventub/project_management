import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { environment } from 'src/environments/environment';
import { AuthService } from './auth.service';
import { AuthResponse } from './auth.models';

describe('AuthService', () => {
    let service: AuthService;
    let httpMock: HttpTestingController;

    const respuesta: AuthResponse = {
        usuarioId: 'u1',
        nombre: 'Admin',
        email: 'admin1@projectmanagement.local',
        token: 'jwt-token',
        expiresAtUtc: '2026-01-01T00:00:00Z'
    };

    beforeEach(() => {
        TestBed.configureTestingModule({
            imports: [HttpClientTestingModule],
            providers: [AuthService]
        });
        service = TestBed.inject(AuthService);
        httpMock = TestBed.inject(HttpTestingController);
        localStorage.clear();
    });

    afterEach(() => {
        httpMock.verify();
        localStorage.clear();
    });

    it('login() posts credentials and stores token + user on success', () => {
        service.login(respuesta.email, 'Admin123!').subscribe(res => {
            expect(res).toEqual(respuesta);
        });

        const req = httpMock.expectOne(`${environment.apiUrl}/auth/login`);
        expect(req.request.method).toBe('POST');
        expect(req.request.body).toEqual({ email: respuesta.email, password: 'Admin123!' });
        req.flush(respuesta);

        expect(service.getToken()).toBe('jwt-token');
        expect(service.isAuthenticated()).toBeTrue();
        expect(service.getCurrentUser()).toEqual(respuesta);
    });

    it('logout() clears the stored token and user', () => {
        localStorage.setItem('pm_auth_token', 'jwt-token');
        localStorage.setItem('pm_auth_user', JSON.stringify(respuesta));

        service.logout();

        expect(service.getToken()).toBeNull();
        expect(service.getCurrentUser()).toBeNull();
        expect(service.isAuthenticated()).toBeFalse();
    });

    it('isAuthenticated() is false when there is no stored token', () => {
        expect(service.isAuthenticated()).toBeFalse();
    });
});
