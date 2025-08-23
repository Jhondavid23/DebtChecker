using DebtCheckerBackend.BLL;
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
    public class DebtsController : ControllerBase
    {
        private readonly IDebtService _debtService;
        private readonly ILogger<DebtsController> _logger;

        public DebtsController(IDebtService debtService, ILogger<DebtsController> logger)
        {
            _debtService = debtService;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las deudas del usuario con filtros y paginación
        /// </summary>
        /// <param name="isPaid">Filtrar por estado de pago</param>
        /// <param name="debtorId">Filtrar por deudor específico</param>
        /// <param name="minAmount">Monto mínimo</param>
        /// <param name="maxAmount">Monto máximo</param>
        /// <param name="fromDate">Fecha desde</param>
        /// <param name="toDate">Fecha hasta</param>
        /// <param name="currency">Moneda</param>
        /// <param name="isOverdue">Solo deudas vencidas</param>
        /// <param name="search">Término de búsqueda</param>
        /// <param name="orderBy">Campo para ordenar</param>
        /// <param name="orderDirection">Dirección del orden (asc/desc)</param>
        /// <param name="page">Número de página</param>
        /// <param name="pageSize">Tamaño de página</param>
        /// <returns>Lista paginada de deudas</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PaginatedResult<DebtDto>>), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetDebts(
            [FromQuery] bool? isPaid = null,
            [FromQuery] int? debtorId = null,
            [FromQuery] decimal? minAmount = null,
            [FromQuery] decimal? maxAmount = null,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null,
            [FromQuery] string? currency = null,
            [FromQuery] bool? isOverdue = null,
            [FromQuery] string? search = null,
            [FromQuery] string orderBy = "CreatedAt",
            [FromQuery] string orderDirection = "desc",
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                    return Unauthorized(new { message = "Token inválido" });

                var filters = new DebtFilters
                {
                    IsPaid = isPaid,
                    DebtorId = debtorId,
                    MinAmount = minAmount,
                    MaxAmount = maxAmount,
                    FromDate = fromDate,
                    ToDate = toDate,
                    Currency = currency,
                    IsOverdue = isOverdue,
                    Search = search,
                    OrderBy = orderBy,
                    OrderDirection = orderDirection,
                    Page = page,
                    PageSize = pageSize
                };

                _logger.LogDebug("Obteniendo deudas para usuario: {UserId} con filtros", userId);

                var result = await _debtService.GetUserDebtsAsync(userId.Value, filters);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener deudas del usuario");
                return StatusCode(500, ApiResponse<PaginatedResult<DebtDto>>.ErrorResult("Error interno del servidor"));
            }
        }

        /// <summary>
        /// Obtiene una deuda específica por ID
        /// </summary>
        /// <param name="id">ID de la deuda</param>
        /// <returns>Detalles de la deuda</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<DebtDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<DebtDto>), 404)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetDebtById(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                    return Unauthorized(new { message = "Token inválido" });

                _logger.LogDebug("Obteniendo deuda: {DebtId} para usuario: {UserId}", id, userId);

                var result = await _debtService.GetDebtByIdAsync(id, userId.Value);

                if (!result.Success)
                {
                    return NotFound(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener deuda por ID: {DebtId}", id);
                return StatusCode(500, ApiResponse<DebtDto>.ErrorResult("Error interno del servidor"));
            }
        }

        /// <summary>
        /// Crea una nueva deuda
        /// </summary>
        /// <param name="request">Datos de la nueva deuda</param>
        /// <returns>Deuda creada</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<DebtDto>), 201)]
        [ProducesResponseType(typeof(ApiResponse<DebtDto>), 400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateDebt([FromBody] CreateDebtRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return BadRequest(ApiResponse<DebtDto>.ErrorResult(
                        "Datos de creación inválidos", errors));
                }

                var userId = GetCurrentUserId();
                if (userId == null)
                    return Unauthorized(new { message = "Token inválido" });

                _logger.LogInformation("Creando nueva deuda para usuario: {UserId}", userId);

                var result = await _debtService.CreateDebtAsync(userId.Value, request);

                if (result.Success)
                {
                    return CreatedAtAction(nameof(GetDebtById), new { id = result.Data?.Id }, result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear deuda");
                return StatusCode(500, ApiResponse<DebtDto>.ErrorResult("Error interno del servidor"));
            }
        }

        /// <summary>
        /// Actualiza una deuda existente
        /// </summary>
        /// <param name="id">ID de la deuda</param>
        /// <param name="request">Datos actualizados</param>
        /// <returns>Deuda actualizada</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<DebtDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<DebtDto>), 400)]
        [ProducesResponseType(typeof(ApiResponse<DebtDto>), 404)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateDebt(int id, [FromBody] UpdateDebtRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return BadRequest(ApiResponse<DebtDto>.ErrorResult(
                        "Datos de actualización inválidos", errors));
                }

                var userId = GetCurrentUserId();
                if (userId == null)
                    return Unauthorized(new { message = "Token inválido" });

                _logger.LogInformation("Actualizando deuda: {DebtId} para usuario: {UserId}", id, userId);

                var result = await _debtService.UpdateDebtAsync(id, userId.Value, request);

                if (!result.Success)
                {
                    if (result.Message.Contains("no encontrada"))
                        return NotFound(result);
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar deuda: {DebtId}", id);
                return StatusCode(500, ApiResponse<DebtDto>.ErrorResult("Error interno del servidor"));
            }
        }

        /// <summary>
        /// Elimina una deuda
        /// </summary>
        /// <param name="id">ID de la deuda</param>
        /// <returns>Confirmación de eliminación</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        [ProducesResponseType(typeof(ApiResponse<bool>), 404)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteDebt(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                    return Unauthorized(new { message = "Token inválido" });

                _logger.LogInformation("Eliminando deuda: {DebtId} para usuario: {UserId}", id, userId);

                var result = await _debtService.DeleteDebtAsync(id, userId.Value);

                if (!result.Success)
                {
                    if (result.Message.Contains("no encontrada"))
                        return NotFound(result);
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar deuda: {DebtId}", id);
                return StatusCode(500, ApiResponse<bool>.ErrorResult("Error interno del servidor"));
            }
        }

        /// <summary>
        /// Marca una deuda como pagada
        /// </summary>
        /// <param name="id">ID de la deuda</param>
        /// <param name="request">Datos del pago (opcional)</param>
        /// <returns>Deuda actualizada</returns>
        [HttpPatch("{id}/pay")]
        [ProducesResponseType(typeof(ApiResponse<DebtDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<DebtDto>), 400)]
        [ProducesResponseType(typeof(ApiResponse<DebtDto>), 404)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PayDebt(int id, [FromBody] PayDebtRequest? request = null)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                    return Unauthorized(new { message = "Token inválido" });

                _logger.LogInformation("Marcando deuda como pagada: {DebtId} para usuario: {UserId}", id, userId);

                var result = await _debtService.PayDebtAsync(id, userId.Value, request);

                if (!result.Success)
                {
                    if (result.Message.Contains("no encontrada"))
                        return NotFound(result);
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al marcar deuda como pagada: {DebtId}", id);
                return StatusCode(500, ApiResponse<DebtDto>.ErrorResult("Error interno del servidor"));
            }
        }

        /// <summary>
        /// Desmarca una deuda como pagada (la vuelve a marcar como pendiente)
        /// </summary>
        /// <param name="id">ID de la deuda</param>
        /// <returns>Deuda actualizada</returns>
        [HttpPatch("{id}/unpay")]
        [ProducesResponseType(typeof(ApiResponse<DebtDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<DebtDto>), 400)]
        [ProducesResponseType(typeof(ApiResponse<DebtDto>), 404)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UnpayDebt(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                    return Unauthorized(new { message = "Token inválido" });

                _logger.LogInformation("Desmarcando deuda como pagada: {DebtId} para usuario: {UserId}", id, userId);

                var result = await _debtService.UnpayDebtAsync(id, userId.Value);

                if (!result.Success)
                {
                    if (result.Message.Contains("no encontrada"))
                        return NotFound(result);
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al desmarcar deuda como pagada: {DebtId}", id);
                return StatusCode(500, ApiResponse<DebtDto>.ErrorResult("Error interno del servidor"));
            }
        }

        /// <summary>
        /// Obtiene las deudas vencidas del usuario
        /// </summary>
        /// <returns>Lista de deudas vencidas</returns>
        [HttpGet("overdue")]
        [ProducesResponseType(typeof(ApiResponse<List<DebtDto>>), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetOverdueDebts()
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                    return Unauthorized(new { message = "Token inválido" });

                _logger.LogDebug("Obteniendo deudas vencidas para usuario: {UserId}", userId);

                var result = await _debtService.GetOverdueDebtsAsync(userId.Value);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener deudas vencidas");
                return StatusCode(500, ApiResponse<List<DebtDto>>.ErrorResult("Error interno del servidor"));
            }
        }

        /// <summary>
        /// Obtiene las deudas más recientes del usuario
        /// </summary>
        /// <param name="count">Número de deudas a obtener (máximo 20)</param>
        /// <returns>Lista de deudas recientes</returns>
        [HttpGet("recent")]
        [ProducesResponseType(typeof(ApiResponse<List<DebtDto>>), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetRecentDebts([FromQuery] int count = 5)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                    return Unauthorized(new { message = "Token inválido" });

                if (count < 1 || count > 20) count = 5;

                _logger.LogDebug("Obteniendo {Count} deudas recientes para usuario: {UserId}", count, userId);

                var result = await _debtService.GetRecentDebtsAsync(userId.Value, count);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener deudas recientes");
                return StatusCode(500, ApiResponse<List<DebtDto>>.ErrorResult("Error interno del servidor"));
            }
        }

        /// <summary>
        /// Busca deudas por término de búsqueda
        /// </summary>
        /// <param name="q">Término de búsqueda</param>
        /// <param name="page">Número de página</param>
        /// <param name="pageSize">Tamaño de página</param>
        /// <returns>Resultados de búsqueda paginados</returns>
        [HttpGet("search")]
        [ProducesResponseType(typeof(ApiResponse<PaginatedResult<DebtDto>>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> SearchDebts(
            [FromQuery] string q,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(q))
                {
                    return BadRequest(ApiResponse<PaginatedResult<DebtDto>>.ErrorResult(
                        "Término de búsqueda requerido",
                        new List<string> { "El parámetro 'q' no puede estar vacío" }));
                }

                var userId = GetCurrentUserId();
                if (userId == null)
                    return Unauthorized(new { message = "Token inválido" });

                _logger.LogDebug("Buscando deudas para usuario: {UserId} con término: {SearchTerm}", userId, q);

                var result = await _debtService.SearchDebtsAsync(userId.Value, q, page, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar deudas con término: {SearchTerm}", q);
                return StatusCode(500, ApiResponse<PaginatedResult<DebtDto>>.ErrorResult("Error interno del servidor"));
            }
        }

        /// <summary>
        /// Obtiene las deudas donde el usuario actual es DEUDOR (debe dinero a otros)
        /// </summary>
        [HttpGet("my-debts")]
        [ProducesResponseType(typeof(ApiResponse<PaginatedResult<DebtDto>>), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetMyDebtsAsDebtor(
            [FromQuery] bool? isPaid = null,
            [FromQuery] decimal? minAmount = null,
            [FromQuery] decimal? maxAmount = null,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null,
            [FromQuery] string? currency = null,
            [FromQuery] bool? isOverdue = null,
            [FromQuery] string? search = null,
            [FromQuery] string orderBy = "CreatedAt",
            [FromQuery] string orderDirection = "desc",
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                    return Unauthorized(new { message = "Token inválido" });

                var filters = new DebtFilters
                {
                    IsPaid = isPaid,
                    MinAmount = minAmount,
                    MaxAmount = maxAmount,
                    FromDate = fromDate,
                    ToDate = toDate,
                    Currency = currency,
                    IsOverdue = isOverdue,
                    Search = search,
                    OrderBy = orderBy,
                    OrderDirection = orderDirection,
                    Page = page,
                    PageSize = pageSize
                };

                _logger.LogDebug("Obteniendo deudas como deudor para usuario: {UserId}", userId);

                var result = await _debtService.GetMyDebtsAsDebtorAsync(userId.Value, filters);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener deudas como deudor");
                return StatusCode(500, ApiResponse<PaginatedResult<DebtDto>>.ErrorResult("Error interno del servidor"));
            }
        }

        /// <summary>
        /// Vista combinada: todas las deudas relacionadas con el usuario logueado
        /// </summary>
        [HttpGet("all-my-debts")]
        [ProducesResponseType(typeof(ApiResponse<Dictionary<string, object>>), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllMyDebts(
            [FromQuery] bool? isPaid = null,
            [FromQuery] string? search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                    return Unauthorized(new { message = "Token inválido" });

                var filters = new DebtFilters
                {
                    IsPaid = isPaid,
                    Search = search,
                    Page = page,
                    PageSize = pageSize
                };

                _logger.LogDebug("Obteniendo todas las deudas relacionadas para usuario: {UserId}", userId);

                var result = await _debtService.GetAllMyDebtsAsync(userId.Value, filters);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las deudas del usuario");
                return StatusCode(500, ApiResponse<Dictionary<string, object>>.ErrorResult("Error interno del servidor"));
            }
        }

        /// <summary>
        /// Obtiene estadísticas de las deudas del usuario
        /// </summary>
        /// <returns>Estadísticas detalladas</returns>
        [HttpGet("statistics")]
        [ProducesResponseType(typeof(ApiResponse<DebtStatistics>), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetStatistics()
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                    return Unauthorized(new { message = "Token inválido" });

                _logger.LogDebug("Obteniendo estadísticas de deudas para usuario: {UserId}", userId);

                var result = await _debtService.GetUserStatisticsAsync(userId.Value);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener estadísticas de deudas");
                return StatusCode(500, ApiResponse<DebtStatistics>.ErrorResult("Error interno del servidor"));
            }
        }

        /// <summary>
        /// Obtiene un resumen completo del usuario (estadísticas + deudas recientes + vencidas)
        /// </summary>
        /// <returns>Resumen completo</returns>
        [HttpGet("summary")]
        [ProducesResponseType(typeof(ApiResponse<Dictionary<string, object>>), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetSummary()
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                    return Unauthorized(new { message = "Token inválido" });

                _logger.LogDebug("Obteniendo resumen completo para usuario: {UserId}", userId);

                var result = await _debtService.GetUserSummaryAsync(userId.Value);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener resumen del usuario");
                return StatusCode(500, ApiResponse<Dictionary<string, object>>.ErrorResult("Error interno del servidor"));
            }
        }

        /// <summary>
        /// Exporta las deudas del usuario en formato JSON o CSV
        /// </summary>
        /// <param name="format">Formato de exportación (json o csv)</param>
        /// <returns>Archivo con las deudas exportadas</returns>
        [HttpGet("export")]
        [ProducesResponseType(typeof(FileResult), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> ExportDebts([FromQuery] string format = "json")
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                    return Unauthorized(new { message = "Token inválido" });

                format = format.ToLower();
                if (format != "json" && format != "csv")
                {
                    return BadRequest(new { message = "Formato no soportado. Use 'json' o 'csv'" });
                }

                _logger.LogInformation("Exportando deudas para usuario: {UserId} en formato: {Format}", userId, format);

                var result = await _debtService.ExportUserDebtsAsync(userId.Value, format);

                if (!result.Success)
                {
                    return BadRequest(result);
                }

                var contentType = format == "json" ? "application/json" : "text/csv";
                var fileName = $"mis-deudas-{DateTime.Now:yyyy-MM-dd}.{format}";

                return File(result.Data!, contentType, fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al exportar deudas en formato: {Format}", format);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene métricas específicas por moneda
        /// </summary>
        /// <returns>Métricas agrupadas por moneda</returns>
        [HttpGet("metrics/by-currency")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetMetricsByCurrency()
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                    return Unauthorized(new { message = "Token inválido" });

                _logger.LogDebug("Obteniendo métricas por moneda para usuario: {UserId}", userId);

                // Este sería un endpoint adicional que podrías implementar
                // Por ahora retornamos un placeholder
                var metrics = new
                {
                    currencies = new[]
                    {
                        new { currency = "COP", totalAmount = 0, pendingAmount = 0, paidAmount = 0, count = 0 },
                        new { currency = "USD", totalAmount = 0, pendingAmount = 0, paidAmount = 0, count = 0 }
                    },
                    lastUpdated = DateTime.UtcNow
                };

                return Ok(new { success = true, data = metrics });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener métricas por moneda");
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