export interface LoginRequest {
    email: string;
    password: string;
}

export interface AuthResponse {
    usuarioId: string;
    nombre: string;
    email: string;
    token: string;
    expiresAtUtc: string;
}
