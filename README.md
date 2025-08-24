# 💰 DebtChecker - Sistema de Gestión de Deudas

<div align="center">

![Version](https://img.shields.io/badge/version-1.0.0-blue.svg)
![.NET](https://img.shields.io/badge/.NET-8.0-purple.svg)
![React](https://img.shields.io/badge/React-19.1.1-blue.svg)
![TypeScript](https://img.shields.io/badge/TypeScript-5.8.3-blue.svg)
![License](https://img.shields.io/badge/license-MIT-green.svg)

**Sistema completo de gestión de deudas con backend robusto en .NET y frontend moderno en React**

[🚀 Instalación](#-instalación) • [📋 Funcionalidades](#-funcionalidades) • [🛠️ Configuración](#️-configuración) • [🐛 Solución de Problemas](#-solución-de-problemas)

</div>

---

## 📖 Descripción

**DebtChecker** es una aplicación web completa para la gestión eficiente de deudas personales. Permite a los usuarios registrar, monitorear y analizar sus deudas con un sistema de reportes avanzado y caché distribuido para alto rendimiento.

### 🎯 Objetivo

Desarrollado como **prueba técnica para la empresa Double V Partners**, este proyecto demuestra la implementación de una arquitectura completa full-stack con las mejores prácticas de desarrollo, incluyendo:

- ✅ **Arquitectura en capas** bien estructurada
- ✅ **Autenticación JWT** segura
- ✅ **Caché distribuido** con AWS DynamoDB
- ✅ **Reportes avanzados** con análisis de tendencias
- ✅ **API REST** documentada con Swagger
- ✅ **Frontend responsive** con React y TypeScript
- ✅ **Validaciones robustas** en ambos lados

---

## 🏗️ Arquitectura del Sistema

```
┌─────────────────┐    HTTP/HTTPS    ┌─────────────────┐
│   Frontend      │ ◄──────────────► │    Backend      │
│   React + TS    │    Port 3000     │    .NET 8 API   │
│   Tailwind CSS  │                  │    Port 7060    │
└─────────────────┘                  └─────────────────┘
                                              │
                                              ▼
                                     ┌─────────────────┐
                                     │   Persistence   │
                                     │ PostgreSQL + DynamoDB │
                                     │   Cache Layer   │
                                     └─────────────────┘
```

---

## 🔧 Stack Tecnológico

### Backend (.NET 8)
- **Framework**: ASP.NET Core Web API 8.0
- **Autenticación**: JWT Bearer Tokens
- **Base de Datos**: PostgreSQL (Entity Framework Core)
- **Caché**: AWS DynamoDB (distribuido)
- **Logging**: ILogger integrado
- **Documentación**: Swagger/OpenAPI
- **Seguridad**: BCrypt para hashing de contraseñas

### Frontend (React 19)
- **Framework**: React 19.1.1 + TypeScript 5.8.3
- **Build Tool**: Vite 7.1.2
- **Styling**: Tailwind CSS 4.1.12
- **Routing**: React Router DOM 7.8.2
- **HTTP Client**: Axios 1.11.0
- **Charts**: Recharts 3.1.2
- **Icons**: Heroicons, Lucide React

---

## 📋 Funcionalidades

### 🖥️ **Backend API - Funcionalidades**

#### 🔐 **Módulo de Autenticación**
- **Registro de usuarios** con validación de email único
- **Login con JWT** y tokens con expiración configurable
- **Logout** con invalidación de sesión
- **Validación de tokens** para verificar autenticidad
- **Hashing seguro de contraseñas** con BCrypt

#### 💰 **Módulo de Gestión de Deudas**
- **CRUD completo** de deudas (Crear, Leer, Actualizar, Eliminar)
- **Filtrado avanzado** por:
  - Estado de pago (pagada/pendiente)
  - Rango de montos (mín/máx)
  - Rango de fechas
  - Moneda
  - Deudor específico
  - Solo vencidas
  - Búsqueda de texto libre
- **Operaciones especiales**:
  - Marcar/desmarcar como pagada
  - Obtener deudas vencidas
  - Obtener deudas recientes
  - Vista de deudas como acreedor vs deudor
- **Paginación y ordenamiento** configurable
- **Exportación** a formatos JSON y CSV
- **Estadísticas detalladas** por usuario

#### 👥 **Módulo de Usuarios**
- **Gestión de perfil** (ver/actualizar información)
- **Cambio de contraseña** seguro
- **Búsqueda de usuarios** por email (para asignar deudores)
- **Historial de actividad** del usuario
- **Eliminación de cuenta** (soft delete)

#### 📊 **Módulo de Reportes Avanzados**
- **Reportes básicos**:
  - Total pagado
  - Saldo pendiente
- **Reportes temporales**:
  - Reportes mensuales detallados
  - Reportes anuales con desglose trimestral
- **Análisis de tendencias**:
  - Tendencias por semana/mes/trimestre/año
  - Comparación entre períodos específicos
  - Insights automáticos
- **Top deudores** y análisis de patrones

#### ☁️ **Sistema de Caché Distribuido**
- **AWS DynamoDB** como backend de caché
- **Expiración automática** de datos
- **Estadísticas de rendimiento** del caché
- **Inicialización automática** de tablas
- **Limpieza de datos expirados**

### 💻 **Frontend - Funcionalidades**

#### 🎨 **Interfaz de Usuario**
- **Diseño responsive** adaptable a todos los dispositivos
- **Dashboard interactivo** con métricas en tiempo real
- **Tema moderno** con Tailwind CSS
- **Navegación intuitiva** con React Router
- **Componentes reutilizables** bien estructurados

#### 🔐 **Autenticación**
- **Formularios de login/registro** con validación
- **Gestión automática de tokens** JWT
- **Protección de rutas** privadas
- **Redirección automática** según estado de autenticación
- **Persistencia de sesión** en localStorage

#### 📊 **Dashboard**
- **Tarjetas de resumen** con estadísticas clave
- **Gráficos interactivos** con Recharts
- **Lista de deudas** con filtros en tiempo real
- **Búsqueda instantánea** por texto
- **Acciones rápidas** (marcar como pagada)

#### 🎛️ **Gestión de Deudas**
- **Formularios dinámicos** para crear
- **Validación en tiempo real** de campos
- **Calendario de vencimientos**
- **Alertas de deudas vencidas**
- **Categorización** por deudor y moneda

#### 📈 **Reportes y Análisis**
- **Gráficos de tendencias** temporales
- **Reportes exportables** (JSON/CSV)
- **Comparativas de períodos**
- **Análisis de patrones** de pago
- **Métricas de rendimiento** personal

---

## 🚀 Instalación

### 📋 Prerequisitos

- **Node.js 18+** - [Descargar aquí](https://nodejs.org/)
- **.NET 8.0 SDK** - [Descargar aquí](https://dotnet.microsoft.com/download/dotnet/8.0)
- **PostgreSQL 12+** - [Descargar aquí](https://www.postgresql.org/download/)
- **Cuenta AWS** (para DynamoDB) - [Crear cuenta](https://aws.amazon.com/)
- **Git** - [Descargar aquí](https://git-scm.com/)

### ⚠️ **IMPORTANTE: Orden de Configuración**

**Sigue este orden exacto para evitar errores:**

1. ✅ Instalar prerequisitos (PostgreSQL, .NET, Node.js)
2. ✅ Clonar repositorio
3. ✅ **Crear base de datos PostgreSQL** usando `SCRIPT_CREACION_BD.sql`
4. ✅ Configurar variables de entorno
5. ✅ Configurar backend y frontend
6. ✅ Ejecutar aplicación

### 1️⃣ Clonar el Repositorio

```bash
git clone https://github.com/Jhondavid23/DebtChecker.git
cd DebtChecker
```

### 2️⃣ Configuración de la Base de Datos

#### 🗄️ **PASO CRÍTICO**: Crear la Base de Datos

⚠️ **ANTES de ejecutar el backend, DEBES crear la base de datos:**

1. **📁 Localizar el script:**
   ```bash
   # El archivo está en la raíz del proyecto:
   SCRIPT_CREACION_BD.sql
   ```

2. **🔧 Ejecutar el script de creación:**
   
   **Opción A: Usando psql (línea de comandos)**
   ```bash
   psql -U postgres -d postgres -f SCRIPT_CREACION_BD.sql
   ```
   
   **Opción B: Usando pgAdmin (interfaz gráfica)**
   - Conectar a PostgreSQL
   - Click derecho en "Databases" → Create → Database
   - O abrir Query Tool y ejecutar el contenido de `SCRIPT_CREACION_BD.sql`
   
   **Opción C: Usando DBeaver u otro cliente**
   - Conectar a PostgreSQL
   - Ejecutar el script SQL

3. **✅ Verificar que la BD se creó correctamente:**
   - Nombre de la base de datos: `debt_management_app`
   - Tablas creadas: `Users`, `Debts` y relacionadas
   - Conexión exitosa con las credenciales configuradas

> 💡 **Nota**: Si no ejecutas este script primero, el backend fallará al inicializar con errores de conexión a la base de datos.

### 3️⃣ Configuración del Backend

```bash
cd DebtCheckerBackend
```

#### Instalar dependencias y compilar
```bash
dotnet restore
dotnet build
```

### 4️⃣ Configuración del Frontend

```bash
cd ../DebtCheckerFront/debt-tracker
npm install
```

---

## 🛠️ Configuración

### ⚙️ Variables de Entorno Requeridas

#### 🔴 **CRÍTICO**: Backend - Variables Obligatorias

⚠️ **Debes configurar estas variables de entorno en tu sistema ANTES de ejecutar el backend:**

```bash
# Autenticación JWT (OBLIGATORIA)
JWT_DEBT_SECRET_KEY=tu_clave_super_secreta_de_al_menos_32_caracteres_muy_larga

# AWS DynamoDB para Caché (OBLIGATORIAS)
AWS_DEBT_ACCESS_KEY=tu_access_key_de_aws
AWS_DEBT_SECRET_KEY=tu_secret_key_de_aws

# Base de Datos PostgreSQL (OBLIGATORIA)
# ⚠️ IMPORTANTE: Crear la BD primero usando SCRIPT_CREACION_BD.sql
DEBT_MANAGEMENT_DB_CONNECTION_STRING=Host=localhost;Database=debt_management_app;Username=postgres;Password=tu_password_postgresql
```

#### 🗄️ **Ejemplos de Cadena de Conexión PostgreSQL:**

```bash
# PostgreSQL local (desarrollo)
Host=localhost;Database=debt_management_app;Username=postgres;Password=123456

# PostgreSQL con puerto personalizado
Host=localhost;Port=5433;Database=debt_management_app;Username=postgres;Password=mi_password

# PostgreSQL remoto con SSL
Host=mi-servidor.com;Database=debt_management_app;Username=mi_usuario;Password=mi_password;SSL Mode=Require

# PostgreSQL en Docker
Host=localhost;Port=5432;Database=debt_management_app;Username=postgres;Password=postgres
```

> 📋 **Componentes de la cadena de conexión:**
> - **Host**: Dirección del servidor PostgreSQL (localhost para local)
> - **Database**: `debt_management_app` (creada con el script SQL)
> - **Username**: Tu usuario PostgreSQL (por defecto: `postgres`)
> - **Password**: La contraseña que configuraste en PostgreSQL
> - **Port**: Puerto de PostgreSQL (por defecto: `5432`)

#### 💡 **Cómo configurar variables de entorno:**

**Windows (PowerShell):**
```powershell
$env:JWT_DEBT_SECRET_KEY="tu_clave_super_secreta_de_al_menos_32_caracteres_muy_larga"
$env:AWS_DEBT_ACCESS_KEY="tu_access_key_de_aws"
$env:AWS_DEBT_SECRET_KEY="tu_secret_key_de_aws"
$env:DEBT_MANAGEMENT_DB_CONNECTION_STRING="Host=localhost;Database=debt_management_app;Username=postgres;Password=tu_password_postgresql"
```

**Windows (Variables del Sistema):**
1. Panel de Control → Sistema → Configuración avanzada del sistema
2. Variables de entorno → Nuevo
3. Agregar cada variable

**macOS/Linux:**
```bash
export JWT_DEBT_SECRET_KEY="tu_clave_super_secreta_de_al_menos_32_caracteres_muy_larga"
export AWS_DEBT_ACCESS_KEY="tu_access_key_de_aws"
export AWS_DEBT_SECRET_KEY="tu_secret_key_de_aws"
export DEBT_MANAGEMENT_DB_CONNECTION_STRING="Host=localhost;Database=debt_management_app;Username=postgres;Password=tu_password_postgresql"
```

#### 🔑 **Obtener credenciales AWS:**
1. Ir a [AWS Console](https://aws.amazon.com/console/)
2. IAM → Usuarios → Crear usuario
3. Adjuntar política: `AmazonDynamoDBFullAccess`
4. Crear clave de acceso → Guardar Access Key y Secret Key

---

## 🚀 Ejecutar la Aplicación

### 🔧 **Backend (.NET API)**

```bash
cd DebtCheckerBackend/DebtCheckerBackend
dotnet run
```

✅ **El backend estará disponible en:**
- HTTPS: `https://localhost:7060`
- HTTP: `http://localhost:5000`
- Swagger UI: `https://localhost:7060/swagger`

### 🎨 **Frontend (React)**

```bash
cd DebtCheckerFront/debt-tracker
npm run dev
```

✅ **El frontend estará disponible en:**
- `http://localhost:3000`

---

## 🌐 Puertos y Comunicación

### 📡 **Configuración de Puertos**

| Aplicación | Puerto | URL | Configuración |
|------------|---------|-----|--------------|
| **Backend API** | `7060` | `https://localhost:7060` | `Properties/launchSettings.json` |
| **Frontend** | `3000` | `http://localhost:3000` | `vite.config.ts` |

### 🔄 **Comunicación Frontend ↔ Backend**

El frontend está configurado para comunicarse con el backend en **dos niveles**:

#### 1. **Configuración Directa** (Archivo: `src/services/api.ts`)
```typescript
const API_BASE_URL = 'https://localhost:7060/api';
```

#### 2. **Proxy de Desarrollo** (Archivo: `vite.config.ts`)
```typescript
server: {
  port: 3000,
  proxy: {
    '/api': {
      target: 'https://localhost:7060',
      changeOrigin: true,
      secure: false
    }
  }
}
```

---

## 🐛 Solución de Problemas

### ❌ **Error: "No se puede conectar al backend"**

**Síntomas:**
- Frontend no puede hacer peticiones
- Errores de CORS
- Timeout de conexión

**Soluciones:**

1. **Verificar que el backend esté ejecutándose:**
   ```bash
   # En DebtCheckerBackend/DebtCheckerBackend
   dotnet run
   ```

2. **Verificar el puerto del backend:**
   - Archivo: `DebtCheckerBackend/DebtCheckerBackend/Properties/launchSettings.json`
   - Buscar: `"applicationUrl": "https://localhost:7060;http://localhost:5000"`

3. **Verificar configuración del frontend:**
   - Archivo: `DebtCheckerFront/debt-tracker/src/services/api.ts`
   - Línea: `const API_BASE_URL = 'https://localhost:7060/api';`
   - Cambiar puerto si es necesario

4. **Verificar proxy de Vite:**
   - Archivo: `DebtCheckerFront/debt-tracker/vite.config.ts`
   - Actualizar la URL target si es necesario

### ❌ **Error: "Variables de entorno no configuradas"**

**Síntomas:**
- Backend se cierra inmediatamente
- Mensaje: "La variable de entorno X no está configurada"

**Solución:**
1. Configurar TODAS las variables obligatorias (ver sección de configuración)
2. Reiniciar la terminal/IDE después de configurar variables
3. Verificar que las variables estén disponibles:
   ```powershell
   echo $env:JWT_DEBT_SECRET_KEY
   ```

### ❌ **Error: "No se puede conectar a DynamoDB"**

**Síntomas:**
- Warnings sobre caché DynamoDB
- Aplicación funciona pero sin caché

**Soluciones:**
1. Verificar credenciales AWS
2. Verificar que la región sea correcta (por defecto: `us-east-1`)
3. El sistema puede funcionar sin caché, pero con menor rendimiento

### ❌ **Error: "No se puede conectar a PostgreSQL"**

**Síntomas:**
- Error al inicializar base de datos
- Timeout de conexión a PostgreSQL
- Error de autenticación con PostgreSQL

**Soluciones:**

1. **Verificar que PostgreSQL esté ejecutándose:**
   ```bash
   # Windows: Verificar en servicios
   services.msc  # Buscar PostgreSQL service
   
   # Linux/Mac: Verificar estado
   sudo systemctl status postgresql
   # o
   pg_ctl status
   ```

2. **Ejecutar el script de base de datos:**
   - 📁 Archivo: `SCRIPT_CREACION_BD.sql` (raíz del proyecto)
   - 🔧 Ejecutar con: `psql -U postgres -d postgres -f SCRIPT_CREACION_BD.sql`
   - ✅ Verificar que la BD `debt_management_app` se creó

3. **Verificar cadena de conexión:**
   ```bash
   # Ejemplo para PostgreSQL local:
   Host=localhost;Database=debt_management_app;Username=postgres;Password=tu_password
   
   # Ejemplo para PostgreSQL remoto:
   Host=tu_servidor.com;Port=5432;Database=debt_management_app;Username=tu_usuario;Password=tu_password;SSL Mode=Require
   ```

4. **Verificar credenciales:**
   - Usuario: `postgres` (por defecto) o tu usuario personalizado
   - Contraseña: La que configuraste durante la instalación
   - Puerto: `5432` (por defecto)
   - Base de datos: `debt_management_app` (creada con el script)

### ❌ **Error: "Puerto ya en uso"**

**Síntomas:**
- Error al iniciar backend/frontend
- Mensaje sobre puerto ocupado

**Soluciones:**
1. **Cambiar puerto del backend:**
   - Archivo: `DebtCheckerBackend/DebtCheckerBackend/Properties/launchSettings.json`
   - Cambiar puerto en `applicationUrl`
   - **IMPORTANTE:** También actualizar `frontend/src/services/api.ts` y `vite.config.ts`

2. **Cambiar puerto del frontend:**
   - Archivo: `DebtCheckerFront/debt-tracker/vite.config.ts`
   - Cambiar `port: 3000` a otro puerto disponible

### 🔧 **Archivos de Configuración Clave**

| Problema | Archivo para Editar |
|----------|-------------------|
| Puerto del backend | `DebtCheckerBackend/DebtCheckerBackend/Properties/launchSettings.json` |
| URL de la API en frontend | `DebtCheckerFront/debt-tracker/src/services/api.ts` |
| Proxy del frontend | `DebtCheckerFront/debt-tracker/vite.config.ts` |
| CORS en backend | `DebtCheckerBackend/DebtCheckerBackend/Program.cs` |
| Configuración AWS | `DebtCheckerBackend/DebtCheckerBackend/appsettings.json` |
| **Script de BD** | `SCRIPT_CREACION_BD.sql` (raíz del proyecto) |

### 🛠️ **Guía Rápida de PostgreSQL**

#### 📦 **Instalación de PostgreSQL:**

**Windows:**
1. Descargar desde [postgresql.org](https://www.postgresql.org/download/windows/)
2. Ejecutar instalador y seguir wizard
3. Anotar la contraseña del usuario `postgres`
4. Puerto por defecto: `5432`

**macOS:**
```bash
# Usando Homebrew
brew install postgresql
brew services start postgresql

# Crear usuario si es necesario
createuser --superuser postgres
```

**Linux (Ubuntu/Debian):**
```bash
sudo apt update
sudo apt install postgresql postgresql-contrib
sudo systemctl start postgresql
sudo systemctl enable postgresql

# Configurar contraseña para postgres
sudo -u postgres psql
postgres=# \password postgres
```

#### 🔍 **Verificar Instalación de PostgreSQL:**

```bash
# Verificar versión
psql --version

# Conectar como postgres
psql -U postgres

# Verificar que el servicio está ejecutándose
# Windows: services.msc (buscar PostgreSQL)
# Linux: sudo systemctl status postgresql
# macOS: brew services list | grep postgresql
```

#### 🐛 **Solución de Problemas Específicos de PostgreSQL:**

**Error: "database does not exist"**
```bash
# Verificar que ejecutaste el script de creación
psql -U postgres -l  # Listar bases de datos
# Debe aparecer: debt_management_app
```

**Error: "password authentication failed"**
```bash
# Verificar credenciales en la cadena de conexión
# Resetear contraseña si es necesario:
sudo -u postgres psql
postgres=# \password postgres
```

**Error: "could not connect to server"**
```bash
# Verificar que PostgreSQL está ejecutándose
sudo systemctl status postgresql  # Linux
services.msc  # Windows
brew services list  # macOS
```

---

## 📚 Documentación de la API

### 🔍 **Swagger UI**

Una vez ejecutado el backend, la documentación completa de la API estará disponible en:

**🌐 https://localhost:7060/swagger**

### 📋 **Endpoints Principales**

#### 🔐 Autenticación
```
POST /api/auth/login         - Iniciar sesión
POST /api/auth/register      - Registrar usuario
POST /api/auth/logout        - Cerrar sesión
GET  /api/auth/validate      - Validar token
```

#### 💰 Gestión de Deudas
```
GET    /api/debts                    - Obtener deudas (con filtros)
POST   /api/debts                    - Crear nueva deuda
GET    /api/debts/{id}               - Obtener deuda por ID
PUT    /api/debts/{id}               - Actualizar deuda
DELETE /api/debts/{id}               - Eliminar deuda
PATCH  /api/debts/{id}/pay           - Marcar como pagada
GET    /api/debts/statistics         - Estadísticas de deudas
GET    /api/debts/export?format=csv  - Exportar deudas
```

#### 📊 Reportes
```
GET /api/reports/monthly/{year}/{month}  - Reporte mensual
GET /api/reports/yearly/{year}           - Reporte anual
GET /api/reports/trends                  - Análisis de tendencias
GET /api/reports/compare                 - Comparar períodos
```

#### 👥 Usuarios
```
GET /api/users/profile           - Obtener perfil
PUT /api/users/profile           - Actualizar perfil
POST /api/users/change-password  - Cambiar contraseña
GET /api/users/search           - Buscar usuarios
```

---

## 🧪 Testing

### 🔧 **Backend Testing**

```bash
cd DebtCheckerBackend
dotnet test
```

### 🎨 **Frontend Testing**

```bash
cd DebtCheckerFront/debt-tracker
npm run lint
npm run build  # Verificar que compile sin errores
```

---

## 📦 Deployment

### 🏭 **Backend (Producción)**

```bash
cd DebtCheckerBackend/DebtCheckerBackend
dotnet publish -c Release -o ./publish
```

### 🌐 **Frontend (Producción)**

```bash
cd DebtCheckerFront/debt-tracker
npm run build
# Los archivos estáticos estarán en ./dist
```

---

## 🤝 Contribuciones

Este proyecto fue desarrollado como **prueba técnica** y demuestra:

- ✅ **Arquitectura limpia** y bien estructurada
- ✅ **Mejores prácticas** de desarrollo
- ✅ **Código mantenible** y escalable
- ✅ **Documentación completa**
- ✅ **Manejo de errores** robusto
- ✅ **Seguridad** implementada correctamente

---

## 📄 Licencia

Este proyecto está bajo la Licencia MIT. Ver el archivo `LICENSE` para más detalles.

---

## 👨‍💻 Autor

**Juan David Díaz Orozco**
- GitHub: [@Jhondavid23](https://github.com/Jhondavid23)
- Proyecto: Prueba Técnica - Sistema de Gestión de Deudas

---

## 🎯 Notas de la Prueba Técnica

### 🏆 **Características Destacadas Implementadas**

1. **🔒 Seguridad Robusta**
   - Autenticación JWT con tokens seguros
   - Hashing de contraseñas con BCrypt
   - CORS configurado correctamente
   - Validación de entrada en ambos lados

2. **📊 Funcionalidades Avanzadas**
   - Sistema de caché distribuido con DynamoDB
   - Reportes con análisis de tendencias
   - Exportación de datos en múltiples formatos
   - Filtrado y búsqueda avanzada

3. **🏗️ Arquitectura Escalable**
   - Patrón Repository implementado
   - Inyección de dependencias
   - Capas bien separadas (API, BLL, DAL, DTO)
   - Logging estructurado

4. **🎨 Frontend Moderno**
   - React 19 con TypeScript
   - Componentes reutilizables
   - Diseño responsive
   - Gestión de estado eficiente

5. **🛠️ Calidad de Código**
   - Documentación XML en API
   - Manejo de errores consistente
   - Validaciones robustas
   - Código limpio y mantenible

---

<div align="center">

### 🚀 **¡Listo para ejecutar!**

Sigue las instrucciones de instalación y configuración para tener el sistema funcionando en minutos.

**¿Problemas?** Revisa la sección de [Solución de Problemas](#-solución-de-problemas) o los archivos de configuración indicados.

</div>
