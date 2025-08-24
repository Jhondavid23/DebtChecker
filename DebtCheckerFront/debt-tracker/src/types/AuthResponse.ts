import type { User } from "./User";

export interface AuthResponse {
  success: boolean;
  message: string;
  token?: string;
  expiresAt?: string;
  user?: User;
  errors?: string[];
}