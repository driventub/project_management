import { HttpErrorResponse, HttpRequest } from '@angular/common/http';
import { of, throwError } from 'rxjs';
import { AuthInterceptor } from './auth.interceptor';
import { AuthService } from './auth.service';
import { Router } from '@angular/router';

describe('AuthInterceptor', () => {
    let authServiceSpy: jasmine.SpyObj<AuthService>;
    let routerSpy: jasmine.SpyObj<Router>;
    let interceptor: AuthInterceptor;

    beforeEach(() => {
        authServiceSpy = jasmine.createSpyObj('AuthService', ['getToken', 'logout']);
        routerSpy = jasmine.createSpyObj('Router', ['navigate']);
        interceptor = new AuthInterceptor(authServiceSpy, routerSpy);
    });

    it('attaches the Authorization header when a token is present', (done) => {
        authServiceSpy.getToken.and.returnValue('jwt-token');
        const req = new HttpRequest('GET', '/api/proyectos');
        const next = { handle: (r: HttpRequest<unknown>) => of(r) } as any;

        interceptor.intercept(req, next).subscribe((clonedReq: any) => {
            expect(clonedReq.headers.get('Authorization')).toBe('Bearer jwt-token');
            done();
        });
    });

    it('does not add an Authorization header when there is no token', (done) => {
        authServiceSpy.getToken.and.returnValue(null);
        const req = new HttpRequest('GET', '/api/proyectos');
        const next = { handle: (r: HttpRequest<unknown>) => of(r) } as any;

        interceptor.intercept(req, next).subscribe((passedReq: any) => {
            expect(passedReq.headers.has('Authorization')).toBeFalse();
            done();
        });
    });

    it('logs out and redirects to /auth/login on a 401 response', (done) => {
        authServiceSpy.getToken.and.returnValue('jwt-token');
        const req = new HttpRequest('GET', '/api/proyectos');
        const error = new HttpErrorResponse({ status: 401 });
        const next = { handle: () => throwError(() => error) } as any;

        interceptor.intercept(req, next).subscribe({
            error: (err) => {
                expect(err).toBe(error);
                expect(authServiceSpy.logout).toHaveBeenCalled();
                expect(routerSpy.navigate).toHaveBeenCalledWith(['/auth/login']);
                done();
            }
        });
    });

    it('does not log out on a non-401 error', (done) => {
        authServiceSpy.getToken.and.returnValue('jwt-token');
        const req = new HttpRequest('GET', '/api/proyectos');
        const error = new HttpErrorResponse({ status: 500 });
        const next = { handle: () => throwError(() => error) } as any;

        interceptor.intercept(req, next).subscribe({
            error: () => {
                expect(authServiceSpy.logout).not.toHaveBeenCalled();
                done();
            }
        });
    });
});
