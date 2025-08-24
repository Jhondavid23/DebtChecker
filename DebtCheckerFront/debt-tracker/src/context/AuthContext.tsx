import React, { createContext, useContext, useState, useEffect } from 'react';
import type { ReactNode } from 'react';
import type { User } from '../types/User';
import type { LoginRequest } from '../types/LoginRequest';
import type { RegisterRequest } from '../types/RegisterRequest';
import { authService } from '../services/api';

interface AuthContextType {
  user: User | null;
  loading: boolean;
  login: (credentials: LoginRequest) => Promise<void>;
  register: (userData: RegisterRequest) => Promise<void>;
  logout: () => void;
  isAuthenticated: boolean;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

interface AuthProviderProps {
  children: ReactNode;
}

export const AuthProvider: React.FC<AuthProviderProps> = ({ children }) => {
  const [user, setUser] = useState<User | null>(null);
  const [loading, setLoading] = useState(true);

  // Verificar token al inicializar
  useEffect(() => {
    const initAuth = async () => {
      const token = localStorage.getItem('token');
      
      if (token) {
        try {
          // Validar token
          await authService.validateToken();
          // Obtener datos del usuario
          const response = await authService.getProfile();
          setUser(response.data);
        } catch (error) {
          console.error('Token inválido:', error);
          localStorage.removeItem('token');
        }
      }
      
      setLoading(false);
    };

    initAuth();
  }, []);

  const login = async (credentials: LoginRequest) => {
    try {
      const response = await authService.login(credentials);
      const { token, user: userData } = response.data;

      if (token && userData) {
        localStorage.setItem('token', token);
        setUser(userData);
      } else {
        throw new Error('Respuesta de login inválida');
      }
    } catch (error: any) {
      const message = error.response?.data?.message || 'Error al iniciar sesión';
      throw new Error(message);
    }
  };

  const register = async (userData: RegisterRequest) => {
    try {
      const response = await authService.register(userData);
      const { token, user: newUser } = response.data;

      if (token && newUser) {
        localStorage.setItem('token', token);
        setUser(newUser);
      } else {
        throw new Error('Respuesta de registro inválida');
      }
    } catch (error: any) {
      const message = error.response?.data?.message || 'Error al registrarse';
      throw new Error(message);
    }
  };

  const logout = () => {
    localStorage.removeItem('token');
    setUser(null);
    // Opcional: llamar al endpoint de logout
    authService.logout().catch(console.error);
  };

  const value: AuthContextType = {
    user,
    loading,
    login,
    register,
    logout,
    isAuthenticated: !!user,
  };

  return (
    <AuthContext.Provider value={value}>
      {children}
    </AuthContext.Provider>
  );
};

// Hook personalizado para usar el contexto
export const useAuth = (): AuthContextType => {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error('useAuth debe usarse dentro de un AuthProvider');
  }
  return context;
};