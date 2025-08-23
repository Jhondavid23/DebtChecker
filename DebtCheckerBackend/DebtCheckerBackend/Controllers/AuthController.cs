using DebtCheckerBackend.BLL.Contrato;
using DebtCheckerBackend.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DebtCheckerBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        /// <summary>
        /// Registra un nuevo usuario en el sistema
        /// </summary>
        /// <param name="request">Datos de registro del usuario</param>
        /// <returns>Respuesta con token JWT si es exitoso</returns>
        [HttpPost("register")]
        [ProducesResponseType(typeof(AuthResponse), 200)]
        [ProducesResponseType(typeof(AuthResponse), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Intento de registro con datos inválidos: {Email}", request.Email);

                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return BadRequest(new AuthResponse
                    {
                        Success = false,
                        Message = "Datos de registro inválidos",
                        Errors = errors
                    });
                }

                _logger.LogInformation("Procesando registro para email: {Email}", request.Email);

                var result = await _authService.RegisterAsync(request);

                if (result.Success)
                {
                    _logger.LogInformation("Registro exitoso para email: {Email}", request.Email);
                    return Ok(result);
                }
                else
                {
                    _logger.LogWarning("Fallo en registro para email: {Email}, Razón: {Message}",
                        request.Email, result.Message);
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error interno durante registro para email: {Email}", request.Email);
                return StatusCode(500, new AuthResponse
                {
                    Success = false,
                    Message = "Error interno del servidor",
                    Errors = new List<string> { "Ocurrió un error inesperado durante el registro" }
                });
            }
        }

        /// <summary>
        /// Autentica un usuario existente
        /// </summary>
        /// <param name="request">Credenciales de login</param>
        /// <returns>Respuesta con token JWT si es exitoso</returns>
        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponse), 200)]
        [ProducesResponseType(typeof(AuthResponse), 400)]
        [ProducesResponseType(typeof(AuthResponse), 401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return BadRequest(new AuthResponse
                    {
                        Success = false,
                        Message = "Datos de login inválidos",
                        Errors = errors
                    });
                }

                _logger.LogInformation("Procesando login para email: {Email}", request.Email);

                var result = await _authService.LoginAsync(request);

                if (result.Success)
                {
                    _logger.LogInformation("Login exitoso para email: {Email}", request.Email);
                    return Ok(result);
                }
                else
                {
                    _logger.LogWarning("Fallo en login para email: {Email}", request.Email);
                    return Unauthorized(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error interno durante login para email: {Email}", request.Email);
                return StatusCode(500, new AuthResponse
                {
                    Success = false,
                    Message = "Error interno del servidor",
                    Errors = new List<string> { "Ocurrió un error inesperado durante el login" }
                });
            }
        }

        /// <summary>
        /// Cierra la sesión del usuario (invalida el token)
        /// </summary>
        /// <returns>Confirmación de logout</returns>
        [HttpPost("logout")]
        [Authorize]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public IActionResult Logout()
        {
            try
            {
                // En una implementación JWT sin estado, el logout es principalmente del lado cliente
                // Aquí podrías implementar una blacklist de tokens si fuera necesario

                var userIdClaim = User.FindFirst("user_id")?.Value;
                _logger.LogInformation("Usuario realizó logout: {UserId}", userIdClaim);

                return Ok(new
                {
                    success = true,
                    message = "Logout exitoso",
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error durante logout");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Valida si un token JWT es válido (útil para el frontend)
        /// </summary>
        /// <returns>Estado de validez del token</returns>
        [HttpGet("validate")]
        [Authorize]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public IActionResult ValidateToken()
        {
            try
            {
                var userIdClaim = User.FindFirst("user_id")?.Value;
                var emailClaim = User.FindFirst("email")?.Value;

                return Ok(new
                {
                    valid = true,
                    userId = userIdClaim,
                    email = emailClaim,
                    expiresAt = User.FindFirst("exp")?.Value,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error durante validación de token");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }
    }
}