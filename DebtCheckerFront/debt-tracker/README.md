# React + TypeScript + Vite

This template provides a minimal setup to get React working in Vite with HMR and some ESLint rules.

Currently, two official plugins are available:

- [@vitejs/plugin-react](https://github.com/vitejs/vite-plugin-react/blob/main/packages/plugin-react) uses [Babel](https://babeljs.io/) for Fast Refresh
- [@vitejs/plugin-react-swc](https://github.com/vitejs/vite-plugin-react/blob/main/packages/plugin-react-swc) uses [SWC](https://swc.rs/) for Fast Refresh

# 💰 Debt Tracker - Sistema de Gestión de Deudas

Una aplicación web moderna y completa para gestionar préstamos y deudas personales, desarrollada con React, TypeScript y Tailwind CSS.

## 🚀 Características Principales

### 📊 Dashboard Intuitivo
- **Tarjetas de Resumen**: Visualización clara del dinero prestado, deudas pendientes y balance neto
- **Sistema de Pestañas**: Separación entre deudas que prestas y deudas que debes
- **Filtros Avanzados**: Filtra por estado (todas, pendientes, pagadas, vencidas)
- **Estados de Carga**: Skeleton loading para mejor experiencia de usuario

### 💳 Gestión de Deudas
- **Crear Deudas**: Formulario intuitivo con búsqueda de usuarios por email
- **Editar Deudas**: Modificación completa de información de deudas existentes
- **Marcar como Pagada**: Funcionalidad para actualizar estado de pago
- **Eliminar Deudas**: Eliminación segura con confirmación
- **Detalles Completos**: Modal con información detallada de cada deuda

### 🔐 Autenticación y Seguridad
- **Registro de Usuarios**: Sistema completo de registro con validaciones
- **Inicio de Sesión**: Autenticación segura con JWT
- **Rutas Protegidas**: Acceso controlado a funcionalidades
- **Manejo de Tokens**: Interceptores automáticos para autorización

### 📱 Interfaz de Usuario
- **Diseño Responsivo**: Optimizado para desktop y móvil
- **UI Moderna**: Interfaz limpia y profesional con Tailwind CSS
- **Notificaciones Toast**: Feedback visual para acciones del usuario
- **Estados de Error**: Manejo elegante de errores y casos edge

## 🛠️ Stack Tecnológico

### Frontend
- **React 19** - Biblioteca principal
- **TypeScript** - Tipado estático
- **Vite** - Herramienta de build ultrarrápida
- **Tailwind CSS 4** - Framework de estilos utility-first
- **React Router DOM** - Navegación y rutas
- **Axios** - Cliente HTTP para API calls

### Herramientas de Desarrollo
- **ESLint** - Linting y calidad de código
- **PostCSS** - Procesamiento de CSS
- **Heroicons & Lucide React** - Iconografía
- **Recharts** - Gráficos y visualizaciones (preparado)

## 📋 Prerequisitos

Antes de comenzar, asegúrate de tener instalado:

- **Node.js** (versión 18 o superior)
- **npm** o **yarn** 
- **Backend API** corriendo en `https://localhost:7060`

## 🚀 Instalación y Configuración

### 1. Clonar el repositorio
```bash
git clone <repository-url>
cd debt-tracker
```

### 2. Instalar dependencias
```bash
npm install
# o
yarn install
```

### 3. Configuración del Backend
Asegúrate de que el backend esté corriendo en `https://localhost:7060`. La aplicación frontend está configurada para conectarse a esta URL.

### 4. Configuración SSL (Importante)
El proyecto está configurado para conectarse a un backend HTTPS. Si tienes problemas con certificados SSL locales, puedes:

- Configurar certificados SSL válidos en tu backend
- O modificar la configuración en `src/services/api.ts` si usas HTTP

### 5. Ejecutar en desarrollo
```bash
npm run dev
# o
yarn dev
```

La aplicación se ejecutará en `http://localhost:3000`

### 6. Build para producción
```bash
npm run build
# o
yarn build
```

## 🏗️ Estructura del Proyecto

```
src/
├── components/           # Componentes reutilizables
│   ├── Common/          # Componentes base (Button, Input, Toast, etc.)
│   ├── Dashboard/       # Componentes específicos del dashboard
│   ├── Layout/          # Componentes de layout (Header, Navigation, etc.)
│   └── Reports/         # Componentes de reportes (futuro)
├── context/             # Context API para estado global
│   └── AuthContext.tsx  # Contexto de autenticación
├── hooks/               # Custom hooks
│   ├── useDebts.tsx     # Hook principal para manejo de deudas
│   └── useTabState.tsx  # Hook para estado de pestañas
├── pages/               # Páginas principales
│   ├── DashboardPage.tsx
│   ├── LoginPage.tsx
│   └── RegisterPage.tsx
├── services/            # Servicios y API calls
│   └── api.ts           # Configuración de Axios y servicios
├── types/               # Definiciones de TypeScript
├── utils/               # Utilidades y helpers
│   └── formatters.ts    # Formateadores de moneda y fecha
└── assets/              # Recursos estáticos
```

## 🔧 Configuraciones Importantes

### API Configuration
El archivo `src/services/api.ts` contiene toda la configuración de conexión con el backend:
- Base URL: `https://localhost:7060/api`
- Timeout: 10 segundos
- Interceptores automáticos para JWT
- Manejo de errores 401 (redirección automática al login)

### Proxy Configuration
El `vite.config.ts` incluye configuración de proxy para desarrollo:
```typescript
server: {
  port: 3000,
  proxy: {
    '/api': {
      target: 'https://localhost:7060',
      changeOrigin: true,
      secure: false  // Para desarrollo local
    }
  }
}
```

### Tailwind Configuration
Configuración personalizada con colores del tema y extensiones específicas del proyecto.

## 🔍 Endpoints Principales

### Autenticación
- `POST /api/auth/login` - Iniciar sesión
- `POST /api/auth/register` - Registrar usuario
- `GET /api/auth/validate` - Validar token

### Deudas
- `GET /api/debts` - Obtener deudas que presté
- `GET /api/debts/my-debts` - Obtener deudas que debo
- `POST /api/Debts` - Crear nueva deuda
- `PUT /api/debts/{id}` - Actualizar deuda
- `DELETE /api/debts/{id}` - Eliminar deuda
- `PATCH /api/Debts/{id}/pay` - Marcar como pagada
- `GET /api/debts/statistics` - Obtener estadísticas

### Usuarios
- `GET /api/Users/search` - Buscar usuarios por email

## 🎨 Características de UI/UX

### Responsive Design
- Grid adaptativo para diferentes pantallas
- Navegación optimizada para móvil
- Modales responsivos

### Loading States
- Skeleton loading para tarjetas de resumen
- Estados de carga para tablas
- Spinners para acciones específicas

### Error Handling
- Notificaciones toast para feedback
- Estados de error amigables
- Validación en tiempo real

### Accessibility
- Semantic HTML
- Keyboard navigation
- ARIA labels apropiados

## 🔐 Seguridad

### Token Management
- JWT almacenado en localStorage
- Interceptores automáticos para autenticación
- Redirección automática en caso de token expirado

### Input Validation
- Validación en frontend y backend
- Sanitización de inputs
- Manejo seguro de errores

## 🚀 Scripts Disponibles

```bash
# Desarrollo
npm run dev          # Inicia servidor de desarrollo

# Build
npm run build        # Construye para producción

# Linting
npm run lint         # Ejecuta ESLint

# Preview
npm run preview      # Preview de build de producción
```

## 🐛 Troubleshooting

### Problemas Comunes

**1. Error de conexión al backend**
- Verifica que el backend esté corriendo en `https://localhost:7060`
- Revisa la configuración SSL
- Confirma que no hay problemas de CORS

**2. Problemas de autenticación**
- Limpia localStorage: `localStorage.clear()`
- Verifica que el token no haya expirado
- Confirma que el backend esté respondiendo correctamente

**3. Errores de build**
- Ejecuta `npm install` para reinstalar dependencias
- Verifica que no hay errores de TypeScript
- Revisa la configuración de Vite

### Logs útiles
```bash
# Ver logs de desarrollo con más detalle
npm run dev -- --debug

# Verificar build
npm run build && npm run preview
```

## 🤝 Contribución

1. Fork el proyecto
2. Crea una rama para tu feature (`git checkout -b feature/AmazingFeature`)
3. Commit tus cambios (`git commit -m 'Add some AmazingFeature'`)
4. Push a la rama (`git push origin feature/AmazingFeature`)
5. Abre un Pull Request

## 📝 Notas para Desarrollo

### Convenciones de Código
- Usar TypeScript estricto
- Componentes funcionales con hooks
- Naming convention: PascalCase para componentes, camelCase para funciones
- Props interface siempre tipadas

### Estructura de Componentes
- Un componente por archivo
- Export default al final
- Props interface antes del componente
- Comentarios JSDoc para componentes complejos

### Estado y Efectos
- Custom hooks para lógica compleja
- useCallback para funciones que se pasan como props
- useMemo para cálculos costosos
- Cleanup apropiado en useEffect

---

**Desarrollado con ❤️ para la gestión eficiente de deudas personales**
