using DebtCheckerBackend.BLL.Contrato;
using DebtCheckerBackend.DTO;
using DebtCheckerBackend.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DebtCheckerBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [Produces("application/json")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserService userService, ILogger<UsersController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene el perfil del usuario actual
        /// </summary>
        /// <returns>Información del usuario autenticado</returns>
        [HttpGet("profile")]
        [ProducesResponseType(typeof(UserDto), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                    return Unauthorized(new { message = "Token inválido" });

                _logger.LogDebug("Obteniendo perfil para usuario: {UserId}", userId);

                var user = await _userService.GetByIdAsync(userId.Value);
                if (user == null)
                {
                    _logger.LogWarning("Perfil no encontrado para usuario: {UserId}", userId);
                    return NotFound(new { message = "Perfil no encontrado" });
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener perfil del usuario");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Actualiza la información del perfil del usuario
        /// </summary>
        /// <param name="updateRequest">Datos a actualizar</param>
        /// <returns>Información actualizada del usuario</returns>
        [HttpPut("profile")]
        [ProducesResponseType(typeof(ApiResponse<UserDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<UserDto>), 400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserRequest updateRequest)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return BadRequest(ApiResponse<UserDto>.ErrorResult(
                        "Datos de actualización inválidos", errors));
                }

                var userId = GetCurrentUserId();
                if (userId == null)
                    return Unauthorized(new { message = "Token inválido" });

                _logger.LogInformation("Actualizando perfil para usuario: {UserId}", userId);

                var result = await _userService.UpdateUserAsync(userId.Value, updateRequest);

                if (result.Success)
                {
                    _logger.LogInformation("Perfil actualizado exitosamente para usuario: {UserId}", userId);
                    return Ok(result);
                }
                else
                {
                    _logger.LogWarning("Fallo al actualizar perfil para usuario: {UserId}, Razón: {Message}",
                        userId, result.Message);
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar perfil del usuario");
                return StatusCode(500, ApiResponse<UserDto>.ErrorResult(
                    "Error interno del servidor",
                    new List<string> { "Ocurrió un error inesperado" }));
            }
        }

        /// <summary>
        /// Cambia la contraseña del usuario actual
        /// </summary>
        /// <param name="changePasswordRequest">Contraseña actual y nueva contraseña</param>
        /// <returns>Resultado de la operación</returns>
        [HttpPost("change-password")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        [ProducesResponseType(typeof(ApiResponse<bool>), 400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest changePasswordRequest)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return BadRequest(ApiResponse<bool>.ErrorResult(
                        "Datos de cambio de contraseña inválidos", errors));
                }

                var userId = GetCurrentUserId();
                if (userId == null)
                    return Unauthorized(new { message = "Token inválido" });

                _logger.LogInformation("Solicitando cambio de contraseña para usuario: {UserId}", userId);

                var result = await _userService.ChangePasswordAsync(
                    userId.Value,
                    changePasswordRequest.CurrentPassword,
                    changePasswordRequest.NewPassword);

                if (result.Success)
                {
                    _logger.LogInformation("Contraseña cambiada exitosamente para usuario: {UserId}", userId);
                    return Ok(result);
                }
                else
                {
                    _logger.LogWarning("Fallo al cambiar contraseña para usuario: {UserId}, Razón: {Message}",
                        userId, result.Message);
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cambiar contraseña del usuario");
                return StatusCode(500, ApiResponse<bool>.ErrorResult(
                    "Error interno del servidor",
                    new List<string> { "Ocurrió un error inesperado" }));
            }
        }

        /// <summary>
        /// Busca usuarios por email (para agregar como deudores)
        /// Solo devuelve información básica por privacidad
        /// </summary>
        /// <param name="email">Email a buscar</param>
        /// <returns>Información básica del usuario si existe</returns>
        [HttpGet("search")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> SearchUserByEmail([FromQuery] string email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                {
                    return BadRequest(new { message = "Email es requerido para la búsqueda" });
                }

                _logger.LogDebug("Buscando usuario por email: {Email}", email);

                var user = await _userService.GetByEmailAsync(email);
                if (user == null)
                {
                    return NotFound(new { message = "Usuario no encontrado" });
                }

                // Solo devolver información básica por privacidad
                var basicInfo = new
                {
                    id = user.Id,
                    fullName = user.FullName,
                    email = user.Email
                };

                return Ok(basicInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar usuario por email: {Email}", email);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene estadísticas del usuario actual
        /// </summary>
        /// <returns>Estadísticas personales del usuario</returns>
        [HttpGet("statistics")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetUserStatistics()
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                    return Unauthorized(new { message = "Token inválido" });

                _logger.LogDebug("Obteniendo estadísticas para usuario: {UserId}", userId);

                // Aquí se integraría con un servicio de estadísticas
                // Por ahora devolvemos un placeholder
                var stats = new
                {
                    totalDebts = 0,
                    pendingDebts = 0,
                    paidDebts = 0,
                    totalAmount = 0.0,
                    pendingAmount = 0.0,
                    lastActivity = DateTime.UtcNow
                };

                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener estadísticas del usuario");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Elimina la cuenta del usuario actual (soft delete)
        /// </summary>
        /// <returns>Confirmación de eliminación</returns>
        [HttpDelete("account")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteAccount()
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                    return Unauthorized(new { message = "Token inválido" });

                _logger.LogWarning("Solicitando eliminación de cuenta para usuario: {UserId}", userId);

                // Implementar lógica de soft delete
                // Por ahora retornamos placeholder
                return Ok(new
                {
                    success = true,
                    message = "Cuenta marcada para eliminación",
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar cuenta del usuario");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene el historial de actividad del usuario
        /// </summary>
        /// <param name="page">Página (por defecto 1)</param>
        /// <param name="pageSize">Tamaño de página (por defecto 10)</param>
        /// <returns>Historial de actividades</returns>
        [HttpGet("activity")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetUserActivity(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                    return Unauthorized(new { message = "Token inválido" });

                if (page < 1) page = 1;
                if (pageSize < 1 || pageSize > 100) pageSize = 10;

                _logger.LogDebug("Obteniendo actividad para usuario: {UserId}, Página: {Page}", userId, page);

                // Placeholder para historial de actividad
                var activity = new
                {
                    page,
                    pageSize,
                    totalItems = 0,
                    totalPages = 0,
                    activities = new List<object>()
                };

                return Ok(activity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener actividad del usuario");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Método helper para obtener el ID del usuario actual del token JWT
        /// </summary>
        /// <returns>ID del usuario o null si no es válido</returns>
        private int? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst("user_id")?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return null;
            }

            return userId;
        }
    }
}