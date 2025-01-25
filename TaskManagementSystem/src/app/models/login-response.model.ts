import { DBUser } from "./db-user.model";

export interface LoginResponse {
    token: string; // The JWT token
    user: Omit<DBUser, 'password'>; // The user details without the password
}