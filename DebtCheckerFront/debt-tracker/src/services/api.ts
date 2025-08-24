// src/services/api.ts - Versión actualizada con búsqueda de usuarios
import axios from 'axios';
import type { AxiosResponse } from 'axios';
import type { User } from '../types/User';
import type { Debt } from '../types/Debt';
import type { DebtStatistics } from '../types/DebtStatistics';
import type { ApiResponse } from '../types/ApiResponse';
import type { PaginatedResult } from '../types/PaginatedResult';
import type { LoginRequest } from '../types/LoginRequest';
import type { RegisterRequest } from '../types/RegisterRequest';
import type { CreateDebtRequest } from '../types/CreateDebtRequest';
import type { AuthResponse } from '../types/AuthResponse';

const API_BASE_URL = 'https://localhost:7060/api';

// Crear instancia de axios
const api = axios.create({
  baseURL: API_BASE_URL,
  timeout: 10000,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Interceptor para agregar token automáticamente
api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('token');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// Interceptor para manejar respuestas y errores
api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      // Token expirado o inválido
      localStorage.removeItem('token');
      window.location.href = '/login';
    }
    return Promise.reject(error);
  }
);

// Servicios de autenticación
export const authService = {
  login: (credentials: LoginRequest): Promise<AxiosResponse<AuthResponse>> => 
    api.post('/auth/login', credentials),
    
  register: (userData: RegisterRequest): Promise<AxiosResponse<AuthResponse>> => 
    api.post('/auth/register', userData),
    
  logout: (): Promise<AxiosResponse> => 
    api.post('/auth/logout'),
    
  validateToken: (): Promise<AxiosResponse> => 
    api.get('/auth/validate'),
    
  getProfile: (): Promise<AxiosResponse<User>> => 
    api.get('/users/profile'),
};

// Servicios de usuarios
export const userService = {
  // Buscar usuarios por email o nombre
  searchUsers: (query: string): Promise<AxiosResponse<User[]>> => 
    api.get(`/Users/search?email=${encodeURIComponent(query)}`),
    
  // Obtener perfil
  getProfile: (): Promise<AxiosResponse<User>> => 
    api.get('/users/profile'),
    
  // Actualizar perfil
  updateProfile: (userData: Partial<User>): Promise<AxiosResponse<ApiResponse<User>>> => 
    api.put('/users/profile', userData),
};

// Servicios de deudas
export const debtService = {
  // Obtener deudas que presté
  getMyDebts: (params?: any): Promise<AxiosResponse<ApiResponse<PaginatedResult<Debt>>>> => 
    api.get('/debts', { params }),
    
  // Obtener deudas que debo
  getDebtsIOwe: (params?: any): Promise<AxiosResponse<ApiResponse<PaginatedResult<Debt>>>> => 
    api.get('/debts/my-debts', { params }),
    
  // Obtener una deuda específica
  getDebtById: (id: number): Promise<AxiosResponse<ApiResponse<Debt>>> => 
    api.get(`/debts/${id}`),
    
  // Crear nueva deuda
  createDebt: (debtData: CreateDebtRequest): Promise<AxiosResponse<ApiResponse<Debt>>> => 
    api.post('/Debts', debtData),
    
  // Actualizar deuda
  updateDebt: (id: number, debtData: CreateDebtRequest): Promise<AxiosResponse<ApiResponse<Debt>>> => 
    api.put(`/debts/${id}`, debtData),
    
  // Eliminar deuda
  deleteDebt: (id: number): Promise<AxiosResponse<ApiResponse<null>>> => 
    api.delete(`/debts/${id}`),
    
  // Marcar deuda como pagada
  markAsPaid: (id: number): Promise<AxiosResponse<ApiResponse<Debt>>> => 
    api.patch(`/Debts/${id}/pay`),
    
  // Desmarcar deuda como pagada (para casos de error)
  unmarkAsPaid: (id: number): Promise<AxiosResponse<ApiResponse<Debt>>> => 
    api.patch(`/debts/${id}/unmark-as-paid`),
    
  // Obtener estadísticas
  getStatistics: (): Promise<AxiosResponse<ApiResponse<DebtStatistics>>> => 
    api.get('/debts/statistics'),
    
  // Exportar deudas
  exportDebts: (format: 'json' | 'csv' | 'excel'): Promise<AxiosResponse<Blob>> => 
    api.get(`/debts/export?format=${format}`, {
      responseType: 'blob'
    }),
    
  // Obtener métricas por moneda
  getMetricsByCurrency: (): Promise<AxiosResponse<ApiResponse<any>>> => 
    api.get('/debts/metrics/by-currency'),
};

// export default api;
// import axios from 'axios';
// import type { AxiosResponse } from 'axios';
// import type { User } from '../types/User';
// import type { Debt } from '../types/Debt';
// import type { DebtStatistics } from '../types/DebtStatistics';
// import type { ApiResponse } from '../types/ApiResponse';
// import type { PaginatedResult } from '../types/PaginatedResult';
// import type { LoginRequest } from '../types/LoginRequest';
// import type { RegisterRequest } from '../types/RegisterRequest';
// import type { CreateDebtRequest } from '../types/CreateDebtRequest';
// import type { AuthResponse } from '../types/AuthResponse';



// const API_BASE_URL = '/api';

// // Crear instancia de axios
// const api = axios.create({
//   baseURL: API_BASE_URL,
//   timeout: 10000,
//   headers: {
//     'Content-Type': 'application/json',
//   },
// });

// // Interceptor para agregar token automáticamente
// api.interceptors.request.use(
//   (config) => {
//     const token = localStorage.getItem('token');
//     if (token) {
//       config.headers.Authorization = `Bearer ${token}`;
//     }
//     return config;
//   },
//   (error) => {
//     return Promise.reject(error);
//   }
// );

// // Interceptor para manejar respuestas y errores
// api.interceptors.response.use(
//   (response) => response,
//   (error) => {
//     if (error.response?.status === 401) {
//       // Token expirado o inválido
//       localStorage.removeItem('token');
//       window.location.href = '/login';
//     }
//     return Promise.reject(error);
//   }
// );

// // Servicios de autenticación
// export const authService = {
//   login: (credentials: LoginRequest): Promise<AxiosResponse<AuthResponse>> => 
//     api.post('/auth/login', credentials),
    
//   register: (userData: RegisterRequest): Promise<AxiosResponse<AuthResponse>> => 
//     api.post('/auth/register', userData),
    
//   logout: (): Promise<AxiosResponse> => 
//     api.post('/auth/logout'),
    
//   validateToken: (): Promise<AxiosResponse> => 
//     api.get('/auth/validate'),
    
//   getProfile: (): Promise<AxiosResponse<User>> => 
//     api.get('/users/profile'),
// };

// // Servicios de deudas
// export const debtService = {
//   // Obtener deudas que presté
//   getMyDebts: (params?: any): Promise<AxiosResponse<ApiResponse<PaginatedResult<Debt>>>> => 
//     api.get('/debts', { params }),
    
//   // Obtener deudas que debo
//   getDebtsIOwe: (params?: any): Promise<AxiosResponse<ApiResponse<PaginatedResult<Debt>>>> => 
//     api.get('/debts/my-debts', { params }),
    
//   // Obtener deuda específica
//   getDebt: (id: number): Promise<AxiosResponse<ApiResponse<Debt>>> => 
//     api.get(`/debts/${id}`),
    
//   // Crear deuda
//   createDebt: (debtData: CreateDebtRequest): Promise<AxiosResponse<ApiResponse<Debt>>> => 
//     api.post('/debts', debtData),
    
//   // Actualizar deuda
//   updateDebt: (id: number, debtData: Partial<CreateDebtRequest>): Promise<AxiosResponse<ApiResponse<Debt>>> => 
//     api.put(`/debts/${id}`, debtData),
    
//   // Eliminar deuda
//   deleteDebt: (id: number): Promise<AxiosResponse<ApiResponse<boolean>>> => 
//     api.delete(`/debts/${id}`),
    
//   // Marcar como pagada
//   payDebt: (id: number): Promise<AxiosResponse<ApiResponse<Debt>>> => 
//     api.patch(`/debts/${id}/pay`),
    
//   // Desmarcar como pagada
//   unpayDebt: (id: number): Promise<AxiosResponse<ApiResponse<Debt>>> => 
//     api.patch(`/debts/${id}/unpay`),
    
//   // Obtener estadísticas
//   getStatistics: (): Promise<AxiosResponse<ApiResponse<DebtStatistics>>> => 
//     api.get('/debts/statistics'),
    
//   // Exportar deudas
//   exportDebts: (format: 'json' | 'csv'): Promise<AxiosResponse<Blob>> => 
//     api.get(`/debts/export?format=${format}`, {
//       responseType: 'blob'
//     }),
// };

// export default api;