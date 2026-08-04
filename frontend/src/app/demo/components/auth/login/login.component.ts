import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { LayoutService } from 'src/app/layout/service/app.layout.service';
import { AuthService } from 'src/app/core/auth/auth.service';

@Component({
    selector: 'app-login',
    templateUrl: './login.component.html',
    styles: [`
        :host ::ng-deep .pi-eye,
        :host ::ng-deep .pi-eye-slash {
            transform:scale(1.6);
            margin-right: 1rem;
            color: var(--primary-color) !important;
        }
    `]
})
export class LoginComponent {

    email!: string;

    password!: string;

    loading = false;

    errorMessage: string | null = null;

    constructor(public layoutService: LayoutService, private authService: AuthService, private router: Router) { }

    login(): void {
        this.errorMessage = null;
        this.loading = true;

        this.authService.login(this.email, this.password).subscribe({
            next: () => {
                this.loading = false;
                this.router.navigate(['/']);
            },
            error: () => {
                this.loading = false;
                this.errorMessage = 'Invalid email or password.';
            }
        });
    }
}
