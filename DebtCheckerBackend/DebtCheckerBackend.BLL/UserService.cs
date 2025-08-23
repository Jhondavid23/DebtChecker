using DebtCheckerBackend.BLL.Contrato;
using DebtCheckerBackend.DAL.Repositorios.Contrato;
using DebtCheckerBackend.DTO;
using DebtCheckerBackend.Model;
using DebtCheckerBackend.Utility;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DebtCheckerBackend.BLL
{
    public class UserService : IUserService
    {
        private readonly IGenericRepository<User> _userRepository;
        private readonly IPasswordService _passwordService;
        private readonly ICacheService _cache; // Cambiado a la interfaz para mejor testabilidad
        private readonly ILogger<UserService> _logger;

        public UserService(
            IGenericRepository<User> userRepository,
            IPasswordService passwordService,
            ICacheService cache,
            ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _passwordService = passwordService;
            _cache = cache;
            _logger = logger;
        }

        public async Task<ApiResponse<UserDto>> RegisterAsync(RegisterRequest request)
        {
            try
            {
                _logger.LogInformation("Iniciando registro de usuario con email: {Email}", request.Email);

                // Verificar si el email ya existe
                if (await ExistsByEmailAsync(request.Email))
                {
                    _logger.LogWarning("Intento de registro con email existente: {Email}", request.Email);
                    return ApiResponse<UserDto>.ErrorResult(
                        "Este email ya está registrado en el sistema",
                        new List<string> { "El email ya existe" }
                    );
                }

                // Crear el nuevo usuario
                var user = new User
                {
                    Email = request.Email.ToLower().Trim(),
                    PasswordHash = _passwordService.HashPassword(request.Password),
                    FirstName = request.FirstName.Trim(),
                    LastName = request.LastName.Trim(),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                // Guardar en base de datos
                var createdUser = await _userRepository.Crear(user);
                _logger.LogInformation("Usuario creado exitosamente con ID: {UserId}", createdUser.Id);

                // Convertir a DTO (sin contraseña)
                var userDto = MapToDto(createdUser);

                // Cachear el usuario para futuras consultas
                await CacheUser(userDto);

                return ApiResponse<UserDto>.SuccessResult(userDto, "Usuario registrado exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar usuario con email: {Email}", request.Email);
                return ApiResponse<UserDto>.ErrorResult(
                    "Error interno del servidor al registrar usuario",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<UserDto?> GetByIdAsync(int id)
        {
            try
            {
                // Intentar obtener del caché primero
                var cacheKey = GenerateCacheKey("user_by_id", id.ToString());
                var cachedUser = await _cache.GetAsync<UserDto>(cacheKey);

                if (cachedUser != null)
                {
                    _logger.LogDebug("Usuario encontrado en caché: {UserId}", id);
                    return cachedUser;
                }

                // Si no está en caché, consultar base de datos
                var user = await _userRepository.Obtener(c => c.Id == id);
                if (user == null)
                {
                    _logger.LogDebug("Usuario no encontrado: {UserId}", id);
                    return null;
                }

                var userDto = MapToDto(user);

                // Guardar en caché
                await CacheUser(userDto);
                _logger.LogDebug("Usuario cargado desde BD y cacheado: {UserId}", id);

                return userDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuario por ID: {UserId}", id);
                return null;
            }
        }

        public async Task<UserDto?> GetByEmailAsync(string email)
        {
            try
            {
                var normalizedEmail = email.ToLower().Trim();

                // Intentar obtener del caché primero
                var cacheKey = GenerateCacheKey("user_by_email", normalizedEmail);
                var cachedUser = await _cache.GetAsync<UserDto>(cacheKey);

                if (cachedUser != null)
                {
                    _logger.LogDebug("Usuario encontrado en caché por email: {Email}", normalizedEmail);
                    return cachedUser;
                }

                // Si no está en caché, consultar base de datos
                var user = await _userRepository.Obtener(u => u.Email == normalizedEmail);
                if (user == null)
                {
                    _logger.LogDebug("Usuario no encontrado por email: {Email}", normalizedEmail);
                    return null;
                }

                var userDto = MapToDto(user);

                // Guardar en caché
                await CacheUser(userDto);
                _logger.LogDebug("Usuario cargado desde BD y cacheado por email: {Email}", normalizedEmail);

                return userDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuario por email: {Email}", email);
                return null;
            }
        }

        public async Task<User?> GetUserEntityByEmailAsync(string email)
        {
            try
            {
                var normalizedEmail = email.ToLower().Trim();
                _logger.LogDebug("Obteniendo entidad de usuario por email: {Email}", normalizedEmail);

                var user = await _userRepository.Obtener(u => u.Email == normalizedEmail);
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener entidad de usuario por email: {Email}", email);
                return null;
            }
        }

        public async Task<User?> GetUserEntityByIdAsync(int id)
        {
            try
            {
                _logger.LogDebug("Obteniendo entidad de usuario por ID: {UserId}", id);

                var user = await _userRepository.Obtener(u => u.Id == id);
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener entidad de usuario por ID: {UserId}", id);
                return null;
            }
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            try
            {
                var normalizedEmail = email.ToLower().Trim();

                // Primero verificar en caché
                var cacheKey = GenerateCacheKey("user_by_email", normalizedEmail);
                var cachedUser = await _cache.GetAsync<UserDto>(cacheKey);

                if (cachedUser != null)
                {
                    return true;
                }

                // Si no está en caché, verificar en base de datos
                var user = await _userRepository.Obtener(u => u.Email == normalizedEmail);
                return user != null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar existencia de usuario por email: {Email}", email);
                return false;
            }
        }

        public async Task<ApiResponse<UserDto>> UpdateUserAsync(int userId, UpdateUserRequest updateRequest)
        {
            try
            {
                _logger.LogInformation("Actualizando usuario: {UserId}", userId);

                // Obtener el usuario existente
                var existingUser = await _userRepository.Obtener(u => u.Id == userId);
                if (existingUser == null)
                {
                    return ApiResponse<UserDto>.ErrorResult("Usuario no encontrado");
                }

                // Actualizar los campos
                existingUser.FirstName = updateRequest.FirstName.Trim();
                existingUser.LastName = updateRequest.LastName.Trim();
                existingUser.UpdatedAt = DateTime.UtcNow;

                // Guardar cambios
                var updated = await _userRepository.Editar(existingUser);
                if (!updated)
                {
                    return ApiResponse<UserDto>.ErrorResult("No se pudo actualizar el usuario");
                }

                var userDto = MapToDto(existingUser);

                // Invalidar caché y cachear la versión actualizada
                await InvalidateUserCache(userId, existingUser.Email);
                await CacheUser(userDto);

                _logger.LogInformation("Usuario actualizado exitosamente: {UserId}", userId);
                return ApiResponse<UserDto>.SuccessResult(userDto, "Usuario actualizado exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar usuario: {UserId}", userId);
                return ApiResponse<UserDto>.ErrorResult(
                    "Error interno del servidor al actualizar usuario",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<bool>> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            try
            {
                _logger.LogInformation("Cambiando contraseña para usuario: {UserId}", userId);

                // Obtener el usuario con contraseña
                var user = await GetUserEntityByIdAsync(userId);
                if (user == null)
                {
                    return ApiResponse<bool>.ErrorResult("Usuario no encontrado");
                }

                // Verificar contraseña actual
                if (!_passwordService.VerifyPassword(currentPassword, user.PasswordHash))
                {
                    _logger.LogWarning("Intento de cambio de contraseña con contraseña actual incorrecta: {UserId}", userId);
                    return ApiResponse<bool>.ErrorResult(
                        "La contraseña actual es incorrecta",
                        new List<string> { "Contraseña actual inválida" }
                    );
                }

                // Actualizar con nueva contraseña
                user.PasswordHash = _passwordService.HashPassword(newPassword);
                user.UpdatedAt = DateTime.UtcNow;

                var updated = await _userRepository.Editar(user);
                if (!updated)
                {
                    return ApiResponse<bool>.ErrorResult("No se pudo cambiar la contraseña");
                }

                _logger.LogInformation("Contraseña cambiada exitosamente para usuario: {UserId}", userId);
                return ApiResponse<bool>.SuccessResult(true, "Contraseña cambiada exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cambiar contraseña para usuario: {UserId}", userId);
                return ApiResponse<bool>.ErrorResult(
                    "Error interno del servidor al cambiar contraseña",
                    new List<string> { ex.Message }
                );
            }
        }

        // Métodos helper privados

        private static UserDto MapToDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                CreatedAt = user.CreatedAt ?? DateTime.UtcNow,
                UpdatedAt = user.UpdatedAt ?? DateTime.UtcNow
            };
        }

        private async Task CacheUser(UserDto userDto)
        {
            try
            {
                var cacheExpiration = TimeSpan.FromMinutes(30);

                // Cachear por ID
                var cacheKeyById = GenerateCacheKey("user_by_id", userDto.Id.ToString());
                await _cache.SetAsync(cacheKeyById, userDto, cacheExpiration);

                // Cachear por email
                var cacheKeyByEmail = GenerateCacheKey("user_by_email", userDto.Email.ToLower());
                await _cache.SetAsync(cacheKeyByEmail, userDto, cacheExpiration);

                _logger.LogDebug("Usuario cacheado: ID={UserId}, Email={Email}", userDto.Id, userDto.Email);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error al cachear usuario: {UserId}", userDto.Id);
                // No lanzar excepción, el caché no es crítico
            }
        }

        private async Task InvalidateUserCache(int userId, string email)
        {
            try
            {
                var cacheKeyById = GenerateCacheKey("user_by_id", userId.ToString());
                var cacheKeyByEmail = GenerateCacheKey("user_by_email", email.ToLower());

                await _cache.DeleteAsync(cacheKeyById);
                await _cache.DeleteAsync(cacheKeyByEmail);

                _logger.LogDebug("Caché invalidado para usuario: ID={UserId}, Email={Email}", userId, email);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error al invalidar caché de usuario: {UserId}", userId);
                // No lanzar excepción, el caché no es crítico
            }
        }

        private static string GenerateCacheKey(string prefix, string identifier)
        {
            return $"{prefix}_{identifier}";
        }

        /// <summary>
        /// Obtiene estadísticas básicas de usuarios (para endpoints de administración)
        /// </summary>
        public async Task<ApiResponse<UserStatistics>> GetUserStatisticsAsync()
        {
            try
            {
                // Este método requeriría consultas más complejas, 
                // por ahora devolvemos estadísticas básicas
                var cacheKey = GenerateCacheKey("user_stats", "general");
                var cachedStats = await _cache.GetAsync<UserStatistics>(cacheKey);

                if (cachedStats != null)
                {
                    return ApiResponse<UserStatistics>.SuccessResult(cachedStats);
                }

                // Aquí implementarías las consultas necesarias
                var stats = new UserStatistics
                {
                    TotalUsers = 0, // Implementar consulta
                    LastUpdated = DateTime.UtcNow
                };

                await _cache.SetAsync(cacheKey, stats, TimeSpan.FromMinutes(15));

                return ApiResponse<UserStatistics>.SuccessResult(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener estadísticas de usuarios");
                return ApiResponse<UserStatistics>.ErrorResult("Error al obtener estadísticas");
            }
        }
    }

    // Clase para estadísticas de usuarios
    public class UserStatistics
    {
        public int TotalUsers { get; set; }
        public int NewUsersThisMonth { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}