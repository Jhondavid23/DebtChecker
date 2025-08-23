using DebtCheckerBackend.BLL.Contrato;
using DebtCheckerBackend.DAL.Repositorios.Contrato;
using DebtCheckerBackend.DTO;
using DebtCheckerBackend.Model;
using DebtCheckerBackend.Utility;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text;

namespace DebtCheckerBackend.BLL
{
    public class DebtService : IDebtService
    {
        private readonly IGenericRepository<Debt> _debtRepository;
        private readonly IGenericRepository<User> _userRepository;
        private readonly ICacheService _cache;
        private readonly ILogger<DebtService> _logger;

        public DebtService(
            IGenericRepository<Debt> debtRepository,
            IGenericRepository<User> userRepository,
            ICacheService cache,
            ILogger<DebtService> logger)
        {
            _debtRepository = debtRepository;
            _userRepository = userRepository;
            _cache = cache;
            _logger = logger;
        }

        public async Task<ApiResponse<DebtDto>> CreateDebtAsync(int userId, CreateDebtRequest request)
        {
            try
            {
                _logger.LogInformation("Creando nueva deuda para usuario: {UserId}", userId);

                // Validación: el monto debe ser positivo
                if (request.Amount <= 0)
                {
                    return ApiResponse<DebtDto>.ErrorResult(
                        "No se pueden crear deudas con valores negativos o cero",
                        new List<string> { "El monto debe ser mayor a 0" });
                }

                // Validar que el usuario existe
                var user = await _userRepository.Obtener(u => u.Id == userId);
                if (user == null)
                {
                    return ApiResponse<DebtDto>.ErrorResult("Usuario no encontrado");
                }

                // Validar que el deudor existe (si se especifica)
                User? debtor = null;
                if (request.DebtorId.HasValue)
                {
                    debtor = await _userRepository.Obtener(u => u.Id == request.DebtorId.Value);
                    if (debtor == null)
                    {
                        return ApiResponse<DebtDto>.ErrorResult("Deudor especificado no encontrado");
                    }
                }

                // Crear la deuda
                var debt = new Debt
                {
                    UserId = userId,
                    DebtorId = request.DebtorId,
                    Title = request.Title.Trim(),
                    Description = request.Description?.Trim(),
                    Amount = request.Amount,
                    Currency = request.Currency?.ToUpper() ?? "COP",
                    IsPaid = false,
                    DueDate = request.DueDate,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                var createdDebt = await _debtRepository.Crear(debt);

                // Invalidar caché del usuario
                await InvalidateUserCache(userId);

                // Mapear a DTO
                var debtDto = MapToDebtDto(createdDebt, user, debtor);

                _logger.LogInformation("Deuda creada exitosamente: {DebtId} para usuario: {UserId}",
                    createdDebt.Id, userId);

                return ApiResponse<DebtDto>.SuccessResult(debtDto, "Deuda creada exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear deuda para usuario: {UserId}", userId);
                return ApiResponse<DebtDto>.ErrorResult(
                    "Error interno del servidor al crear deuda",
                    new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<DebtDto>> GetDebtByIdAsync(int debtId, int userId)
        {
            try
            {
                _logger.LogDebug("Obteniendo deuda: {DebtId} para usuario: {UserId}", debtId, userId);

                // Intentar obtener del caché primero
                var cacheKey = $"debt_{debtId}";
                var cachedDebt = await _cache.GetAsync<DebtDto>(cacheKey);

                if (cachedDebt != null && cachedDebt.UserId == userId)
                {
                    return ApiResponse<DebtDto>.SuccessResult(cachedDebt);
                }

                // Si no está en caché, consultar base de datos
                var debt = await _debtRepository.Obtener(d => d.Id == debtId && d.UserId == userId);

                if (debt == null)
                {
                    return ApiResponse<DebtDto>.ErrorResult("Deuda no encontrada");
                }

                // Obtener información del usuario y deudor por separado
                var user = await _userRepository.Obtener(u => u.Id == debt.UserId);
                User? debtor = null;
                if (debt.DebtorId.HasValue)
                {
                    debtor = await _userRepository.Obtener(u => u.Id == debt.DebtorId.Value);
                }

                var debtDto = MapToDebtDto(debt, user, debtor);

                // Cachear resultado por 15 minutos
                await _cache.SetAsync(cacheKey, debtDto, TimeSpan.FromMinutes(15));

                return ApiResponse<DebtDto>.SuccessResult(debtDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener deuda: {DebtId} para usuario: {UserId}", debtId, userId);
                return ApiResponse<DebtDto>.ErrorResult("Error interno del servidor");
            }
        }

        public async Task<ApiResponse<DebtDto>> UpdateDebtAsync(int debtId, int userId, UpdateDebtRequest request)
        {
            try
            {
                _logger.LogInformation("Actualizando deuda: {DebtId} para usuario: {UserId}", debtId, userId);

                // Validación: el monto debe ser positivo
                if (request.Amount <= 0)
                {
                    return ApiResponse<DebtDto>.ErrorResult(
                        "No se pueden crear deudas con valores negativos o cero",
                        new List<string> { "El monto debe ser mayor a 0" });
                }

                // Obtener la deuda existente
                var debt = await _debtRepository.Obtener(d => d.Id == debtId && d.UserId == userId);

                if (debt == null)
                {
                    return ApiResponse<DebtDto>.ErrorResult("Deuda no encontrada");
                }

                // Validación: No se pueden modificar deudas pagadas
                if (debt.IsPaid == true)
                {
                    return ApiResponse<DebtDto>.ErrorResult(
                        "No se pueden modificar deudas que ya han sido marcadas como pagadas",
                        new List<string> { "Deuda ya pagada" });
                }

                // Validar deudor (si se especifica)
                User? debtor = null;
                if (request.DebtorId.HasValue)
                {
                    debtor = await _userRepository.Obtener(u => u.Id == request.DebtorId.Value);
                    if (debtor == null)
                    {
                        return ApiResponse<DebtDto>.ErrorResult("Deudor especificado no encontrado");
                    }
                }

                // Actualizar campos
                debt.Title = request.Title.Trim();
                debt.Description = request.Description?.Trim();
                debt.Amount = request.Amount;
                debt.Currency = request.Currency?.ToUpper() ?? "COP";
                debt.DebtorId = request.DebtorId;
                debt.DueDate = request.DueDate;
                debt.UpdatedAt = DateTime.UtcNow;

                var updated = await _debtRepository.Editar(debt);
                if (!updated)
                {
                    return ApiResponse<DebtDto>.ErrorResult("No se pudo actualizar la deuda");
                }

                // Invalidar caché
                await InvalidateDebtCache(debtId);
                await InvalidateUserCache(userId);

                var user = await _userRepository.Obtener(u => u.Id == userId);
                var debtDto = MapToDebtDto(debt, user, debtor);

                _logger.LogInformation("Deuda actualizada exitosamente: {DebtId}", debtId);
                return ApiResponse<DebtDto>.SuccessResult(debtDto, "Deuda actualizada exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar deuda: {DebtId} para usuario: {UserId}", debtId, userId);
                return ApiResponse<DebtDto>.ErrorResult("Error interno del servidor");
            }
        }

        public async Task<ApiResponse<bool>> DeleteDebtAsync(int debtId, int userId)
        {
            try
            {
                _logger.LogInformation("Eliminando deuda: {DebtId} para usuario: {UserId}", debtId, userId);

                var debt = await _debtRepository.Obtener(d => d.Id == debtId && d.UserId == userId);
                if (debt == null)
                {
                    return ApiResponse<bool>.ErrorResult("Deuda no encontrada");
                }

                var deleted = await _debtRepository.Eliminar(debt);
                if (!deleted)
                {
                    return ApiResponse<bool>.ErrorResult("No se pudo eliminar la deuda");
                }

                // Invalidar caché
                await InvalidateDebtCache(debtId);
                await InvalidateUserCache(userId);

                _logger.LogInformation("Deuda eliminada exitosamente: {DebtId}", debtId);
                return ApiResponse<bool>.SuccessResult(true, "Deuda eliminada exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar deuda: {DebtId} para usuario: {UserId}", debtId, userId);
                return ApiResponse<bool>.ErrorResult("Error interno del servidor");
            }
        }

        public async Task<ApiResponse<DebtDto>> PayDebtAsync(int debtId, int userId, PayDebtRequest? request = null)
        {
            try
            {
                _logger.LogInformation("Marcando deuda como pagada: {DebtId} para usuario: {UserId}", debtId, userId);

                var debt = await _debtRepository.Obtener(d => d.Id == debtId && d.UserId == userId);

                if (debt == null)
                {
                    return ApiResponse<DebtDto>.ErrorResult("Deuda no encontrada");
                }

                if (debt.IsPaid == true)
                {
                    return ApiResponse<DebtDto>.ErrorResult("La deuda ya está marcada como pagada");
                }

                // Marcar como pagada
                debt.IsPaid = true;
                debt.PaidAt = request?.PaidAt ?? DateTime.UtcNow;
                debt.UpdatedAt = DateTime.UtcNow;

                var updated = await _debtRepository.Editar(debt);
                if (!updated)
                {
                    return ApiResponse<DebtDto>.ErrorResult("No se pudo marcar la deuda como pagada");
                }

                // Invalidar caché
                await InvalidateDebtCache(debtId);
                await InvalidateUserCache(userId);

                // Obtener información completa para el DTO
                var user = await _userRepository.Obtener(u => u.Id == userId);
                User? debtor = null;
                if (debt.DebtorId.HasValue)
                {
                    debtor = await _userRepository.Obtener(u => u.Id == debt.DebtorId.Value);
                }

                var debtDto = MapToDebtDto(debt, user, debtor);

                _logger.LogInformation("Deuda marcada como pagada exitosamente: {DebtId}", debtId);
                return ApiResponse<DebtDto>.SuccessResult(debtDto, "Deuda marcada como pagada exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al marcar deuda como pagada: {DebtId} para usuario: {UserId}", debtId, userId);
                return ApiResponse<DebtDto>.ErrorResult("Error interno del servidor");
            }
        }

        public async Task<ApiResponse<DebtDto>> UnpayDebtAsync(int debtId, int userId)
        {
            try
            {
                _logger.LogInformation("Desmarcando deuda como pagada: {DebtId} para usuario: {UserId}", debtId, userId);

                var debt = await _debtRepository.Obtener(d => d.Id == debtId && d.UserId == userId);

                if (debt == null)
                {
                    return ApiResponse<DebtDto>.ErrorResult("Deuda no encontrada");
                }

                if (debt.IsPaid == false)
                {
                    return ApiResponse<DebtDto>.ErrorResult("La deuda no está marcada como pagada");
                }

                // Desmarcar como pagada
                debt.IsPaid = false;
                debt.PaidAt = null;
                debt.UpdatedAt = DateTime.UtcNow;

                var updated = await _debtRepository.Editar(debt);
                if (!updated)
                {
                    return ApiResponse<DebtDto>.ErrorResult("No se pudo desmarcar la deuda como pagada");
                }

                // Invalidar caché
                await InvalidateDebtCache(debtId);
                await InvalidateUserCache(userId);

                // Obtener información completa para el DTO
                var user = await _userRepository.Obtener(u => u.Id == userId);
                User? debtor = null;
                if (debt.DebtorId.HasValue)
                {
                    debtor = await _userRepository.Obtener(u => u.Id == debt.DebtorId.Value);
                }

                var debtDto = MapToDebtDto(debt, user, debtor);

                _logger.LogInformation("Deuda desmarcada como pagada exitosamente: {DebtId}", debtId);
                return ApiResponse<DebtDto>.SuccessResult(debtDto, "Deuda desmarcada como pagada exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al desmarcar deuda como pagada: {DebtId} para usuario: {UserId}", debtId, userId);
                return ApiResponse<DebtDto>.ErrorResult("Error interno del servidor");
            }
        }


        public async Task<ApiResponse<PaginatedResult<DebtDto>>> GetUserDebtsAsync(int userId, DebtFilters filters)
        {
            try
            {
                _logger.LogDebug("Obteniendo deudas para usuario: {UserId} con filtros", userId);

                // Validar paginación
                if (filters.Page < 1) filters.Page = 1;
                if (filters.PageSize < 1 || filters.PageSize > 100) filters.PageSize = 10;

            
                var query = await _debtRepository.Consultar(d => d.UserId == userId);

                // Aplicar filtros a nivel de base de datos
                if (filters.IsPaid.HasValue)
                    query = query.Where(d => d.IsPaid == filters.IsPaid.Value);

                if (filters.DebtorId.HasValue)
                    query = query.Where(d => d.DebtorId == filters.DebtorId.Value);

                if (filters.MinAmount.HasValue)
                    query = query.Where(d => d.Amount >= filters.MinAmount.Value);

                if (filters.MaxAmount.HasValue)
                    query = query.Where(d => d.Amount <= filters.MaxAmount.Value);

                if (filters.FromDate.HasValue)
                    query = query.Where(d => d.CreatedAt >= filters.FromDate.Value);

                if (filters.ToDate.HasValue)
                    query = query.Where(d => d.CreatedAt <= filters.ToDate.Value);

                if (!string.IsNullOrEmpty(filters.Currency))
                    query = query.Where(d => d.Currency == filters.Currency.ToUpper());

                if (filters.IsOverdue == true)
                    query = query.Where(d => (!d.IsPaid.HasValue || !d.IsPaid.Value) &&
                                       d.DueDate.HasValue &&
                                       d.DueDate.Value < DateTime.UtcNow);

                if (!string.IsNullOrEmpty(filters.Search))
                {
                    var searchTerm = filters.Search.ToLower();
                    query = query.Where(d => d.Title.ToLower().Contains(searchTerm) ||
                                       (d.Description != null && d.Description.ToLower().Contains(searchTerm)));
                }

                // Aplicar ordenamiento
                query = ApplyOrderingToQuery(query, filters.OrderBy, filters.OrderDirection);

                // Obtener total para paginación 
                var totalItems = query.Count();
                var totalPages = (int)Math.Ceiling((double)totalItems / filters.PageSize);

                // Aplicar paginación y ejecutar consulta
                var pagedDebts = query
                    .Skip((filters.Page - 1) * filters.PageSize)
                    .Take(filters.PageSize)
                    .ToList();

                // Obtener todos los usuarios necesarios en una sola consulta
                var debtorIds = pagedDebts.Where(d => d.DebtorId.HasValue)
                                          .Select(d => d.DebtorId.Value)
                                          .Distinct()
                                          .ToList();

                var user = await _userRepository.Obtener(u => u.Id == userId);
                var debtors = new Dictionary<int, User>();

                if (debtorIds.Any())
                {
                    var debtorUsers = await _userRepository.Consultar(u => debtorIds.Contains(u.Id));
                    debtors = debtorUsers.ToDictionary(u => u.Id, u => u);
                }

                // Mapear a DTOs
                var debtDtos = pagedDebts.Select(debt =>
                {
                    debtors.TryGetValue(debt.DebtorId ?? 0, out var debtor);
                    return MapToDebtDto(debt, user, debtor);
                }).ToList();

                var result = new PaginatedResult<DebtDto>
                {
                    Items = debtDtos,
                    TotalItems = totalItems,
                    Page = filters.Page,
                    PageSize = filters.PageSize,
                    TotalPages = totalPages,
                    HasNextPage = filters.Page < totalPages,
                    HasPreviousPage = filters.Page > 1
                };

                return ApiResponse<PaginatedResult<DebtDto>>.SuccessResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener deudas para usuario: {UserId}", userId);
                return ApiResponse<PaginatedResult<DebtDto>>.ErrorResult("Error interno del servidor");
            }
        }

        public async Task<ApiResponse<List<DebtDto>>> GetOverdueDebtsAsync(int userId)
        {
            try
            {
                _logger.LogDebug("Obteniendo deudas vencidas para usuario: {UserId}", userId);

                var allDebts = await _debtRepository.Consultar(d => d.UserId == userId);
                var overdueDebts = allDebts
                    .Where(d => (!d.IsPaid.HasValue || !d.IsPaid.Value) &&
                               d.DueDate.HasValue &&
                               d.DueDate.Value < DateTime.UtcNow)
                    .OrderBy(d => d.DueDate)
                    .ToList();

                var debtDtos = new List<DebtDto>();
                var user = await _userRepository.Obtener(u => u.Id == userId);

                foreach (var debt in overdueDebts)
                {
                    User? debtor = null;
                    if (debt.DebtorId.HasValue)
                    {
                        debtor = await _userRepository.Obtener(u => u.Id == debt.DebtorId.Value);
                    }
                    var dto = MapToDebtDto(debt, user, debtor);
                    debtDtos.Add(dto);
                }

                return ApiResponse<List<DebtDto>>.SuccessResult(debtDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener deudas vencidas para usuario: {UserId}", userId);
                return ApiResponse<List<DebtDto>>.ErrorResult("Error interno del servidor");
            }
        }

        public async Task<ApiResponse<DebtStatistics>> GetUserStatisticsAsync(int userId)
        {
            try
            {
                _logger.LogDebug("Obteniendo estadísticas para usuario: {UserId}", userId);

                // Intentar obtener del caché primero
                var cacheKey = $"user_debt_stats_{userId}";
                var cachedStats = await _cache.GetAsync<DebtStatistics>(cacheKey);

                if (cachedStats != null)
                {
                    return ApiResponse<DebtStatistics>.SuccessResult(cachedStats);
                }

                // Obtener todas las deudas del usuario
                var allDebts = await _debtRepository.Consultar(d => d.UserId == userId);
                var debts = allDebts.ToList();

                var stats = new DebtStatistics
                {
                    TotalDebts = debts.Count,
                    PendingDebts = debts.Count(d => !d.IsPaid.HasValue || !d.IsPaid.Value),
                    PaidDebts = debts.Count(d => d.IsPaid.HasValue && d.IsPaid.Value),
                    OverdueDebts = debts.Count(d => (!d.IsPaid.HasValue || !d.IsPaid.Value) && d.DueDate.HasValue && d.DueDate.Value < DateTime.UtcNow),
                    TotalAmount = debts.Sum(d => d.Amount),
                    PendingAmount = debts.Where(d => !d.IsPaid.HasValue || !d.IsPaid.Value).Sum(d => d.Amount),
                    PaidAmount = debts.Where(d => d.IsPaid.HasValue && d.IsPaid.Value).Sum(d => d.Amount),
                    OverdueAmount = debts.Where(d => (!d.IsPaid.HasValue || !d.IsPaid.Value) && d.DueDate.HasValue && d.DueDate.Value < DateTime.UtcNow).Sum(d => d.Amount),
                    Currency = "COP",
                    LastUpdated = DateTime.UtcNow
                };

                // Cachear por 10 minutos
                await _cache.SetAsync(cacheKey, stats, TimeSpan.FromMinutes(10));

                return ApiResponse<DebtStatistics>.SuccessResult(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener estadísticas para usuario: {UserId}", userId);
                return ApiResponse<DebtStatistics>.ErrorResult("Error interno del servidor");
            }
        }

        // Método helper para trabajar con IQueryable<Debt> 
        private static IQueryable<Debt> ApplyOrderingToQuery(IQueryable<Debt> query, string orderBy, string orderDirection)
        {
            var ascending = orderDirection.ToLower() == "asc";

            return orderBy.ToLower() switch
            {
                "amount" => ascending ? query.OrderBy(d => d.Amount) : query.OrderByDescending(d => d.Amount),
                "duedate" => ascending ? query.OrderBy(d => d.DueDate) : query.OrderByDescending(d => d.DueDate),
                "title" => ascending ? query.OrderBy(d => d.Title) : query.OrderByDescending(d => d.Title),
                "ispaid" => ascending ? query.OrderBy(d => d.IsPaid) : query.OrderByDescending(d => d.IsPaid),
                "updatedat" => ascending ? query.OrderBy(d => d.UpdatedAt) : query.OrderByDescending(d => d.UpdatedAt),
                _ => ascending ? query.OrderBy(d => d.CreatedAt) : query.OrderByDescending(d => d.CreatedAt)
            };
        }

        public async Task<ApiResponse<byte[]>> ExportUserDebtsAsync(int userId, string format = "json")
        {
            try
            {
                _logger.LogInformation("Exportando deudas para usuario: {UserId} en formato: {Format}", userId, format);

                var filters = new DebtFilters { Page = 1, PageSize = 1000 };
                var debtsResult = await GetUserDebtsAsync(userId, filters);

                if (!debtsResult.Success)
                {
                    return ApiResponse<byte[]>.ErrorResult("Error al obtener deudas para exportar");
                }

                byte[] exportData;
                switch (format.ToLower())
                {
                    case "json":
                        var jsonString = JsonSerializer.Serialize(debtsResult.Data?.Items, new JsonSerializerOptions
                        {
                            WriteIndented = true
                        });
                        exportData = Encoding.UTF8.GetBytes(jsonString);
                        break;

                    case "csv":
                        exportData = GenerateCsv(debtsResult.Data?.Items ?? new List<DebtDto>());
                        break;

                    default:
                        return ApiResponse<byte[]>.ErrorResult("Formato no soportado. Use 'json' o 'csv'");
                }

                return ApiResponse<byte[]>.SuccessResult(exportData, $"Datos exportados exitosamente en formato {format.ToUpper()}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al exportar deudas para usuario: {UserId}", userId);
                return ApiResponse<byte[]>.ErrorResult("Error interno del servidor");
            }
        }

        // Métodos requeridos por la interfaz 
        // Obtener las deudas recientes del usuario
        public async Task<ApiResponse<List<DebtDto>>> GetRecentDebtsAsync(int userId, int count = 5)
        {
            var filters = new DebtFilters
            {
                Page = 1,
                PageSize = count,
                OrderBy = "CreatedAt",
                OrderDirection = "desc"
            };

            var result = await GetUserDebtsAsync(userId, filters);
            return ApiResponse<List<DebtDto>>.SuccessResult(result.Data?.Items ?? new List<DebtDto>());
        }

        public async Task<ApiResponse<Dictionary<string, object>>> GetUserSummaryAsync(int userId)
        {
            var statsResult = await GetUserStatisticsAsync(userId);
            var recentResult = await GetRecentDebtsAsync(userId, 3);
            var overdueResult = await GetOverdueDebtsAsync(userId);

            var summary = new Dictionary<string, object>
            {
                ["statistics"] = statsResult.Data,
                ["recentDebts"] = recentResult.Success ? recentResult.Data : new List<DebtDto>(),
                ["overdueDebts"] = overdueResult.Success ? overdueResult.Data?.Take(5).ToList() : new List<DebtDto>(),
                ["lastUpdated"] = DateTime.UtcNow
            };

            return ApiResponse<Dictionary<string, object>>.SuccessResult(summary);
        }

        // Método para buscar deudas por término de búsqueda
        public async Task<ApiResponse<PaginatedResult<DebtDto>>> SearchDebtsAsync(int userId, string searchTerm, int page = 1, int pageSize = 10)
        {
            var filters = new DebtFilters
            {
                Search = searchTerm,
                Page = page,
                PageSize = pageSize
            };

            return await GetUserDebtsAsync(userId, filters);
        }

        // Obtiene las deudas donde el usuario actual es el DEUDOR (debe dinero a otros)
        public async Task<ApiResponse<PaginatedResult<DebtDto>>> GetMyDebtsAsDebtorAsync(int userId, DebtFilters filters)
        {
            try
            {
                _logger.LogDebug("Obteniendo deudas como deudor para usuario: {UserId}", userId);

                // Validar paginación
                if (filters.Page < 1) filters.Page = 1;
                if (filters.PageSize < 1 || filters.PageSize > 100) filters.PageSize = 10;

                // Consultar deudas donde el usuario es el DEUDOR
                var query = await _debtRepository.Consultar(d => d.DebtorId == userId);

                // Aplicar filtros (misma lógica que GetUserDebtsAsync)
                if (filters.IsPaid.HasValue)
                    query = query.Where(d => d.IsPaid == filters.IsPaid.Value);

                if (filters.MinAmount.HasValue)
                    query = query.Where(d => d.Amount >= filters.MinAmount.Value);

                if (filters.MaxAmount.HasValue)
                    query = query.Where(d => d.Amount <= filters.MaxAmount.Value);

                if (filters.FromDate.HasValue)
                    query = query.Where(d => d.CreatedAt >= filters.FromDate.Value);

                if (filters.ToDate.HasValue)
                    query = query.Where(d => d.CreatedAt <= filters.ToDate.Value);

                if (!string.IsNullOrEmpty(filters.Currency))
                    query = query.Where(d => d.Currency == filters.Currency.ToUpper());

                if (filters.IsOverdue == true)
                    query = query.Where(d => (!d.IsPaid.HasValue || !d.IsPaid.Value) &&
                                           d.DueDate.HasValue &&
                                           d.DueDate.Value < DateTime.UtcNow);

                if (!string.IsNullOrEmpty(filters.Search))
                {
                    var searchTerm = filters.Search.ToLower();
                    query = query.Where(d => d.Title.ToLower().Contains(searchTerm) ||
                                           (d.Description != null && d.Description.ToLower().Contains(searchTerm)));
                }

                // Aplicar ordenamiento
                query = ApplyOrderingToQuery(query, filters.OrderBy, filters.OrderDirection);

                // Obtener total para paginación
                var totalItems = query.Count();
                var totalPages = (int)Math.Ceiling((double)totalItems / filters.PageSize);

                // Aplicar paginación y ejecutar consulta
                var pagedDebts = query
                    .Skip((filters.Page - 1) * filters.PageSize)
                    .Take(filters.PageSize)
                    .ToList();

                // Obtener información de los acreedores (owners)
                var ownerIds = pagedDebts.Select(d => d.UserId).Distinct().ToList();
                var owners = new Dictionary<int, User>();

                if (ownerIds.Any())
                {
                    var ownerUsers = await _userRepository.Consultar(u => ownerIds.Contains(u.Id));
                    owners = ownerUsers.ToDictionary(u => u.Id, u => u);
                }

                // Obtener información del deudor (el usuario actual)
                var debtor = await _userRepository.Obtener(u => u.Id == userId);

                // Mapear a DTOs
                var debtDtos = pagedDebts.Select(debt =>
                {
                    owners.TryGetValue(debt.UserId, out var owner);
                    return MapToDebtDto(debt, owner, debtor);
                }).ToList();

                var result = new PaginatedResult<DebtDto>
                {
                    Items = debtDtos,
                    TotalItems = totalItems,
                    Page = filters.Page,
                    PageSize = filters.PageSize,
                    TotalPages = totalPages,
                    HasNextPage = filters.Page < totalPages,
                    HasPreviousPage = filters.Page > 1
                };

                return ApiResponse<PaginatedResult<DebtDto>>.SuccessResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener deudas como deudor para usuario: {UserId}", userId);
                return ApiResponse<PaginatedResult<DebtDto>>.ErrorResult("Error interno del servidor");
            }
        }

        // Vista combinada: todas las deudas relacionadas conmigo (que presto + que debo)
        public async Task<ApiResponse<Dictionary<string, object>>> GetAllMyDebtsAsync(int userId, DebtFilters filters)
        {
            try
            {
                // Obtener deudas que he prestado (soy acreedor)
                var debtsILent = await GetUserDebtsAsync(userId, filters);

                // Obtener deudas que debo (soy deudor)
                var debtsIOwn = await GetMyDebtsAsDebtorAsync(userId, filters);

                var result = new Dictionary<string, object>
                {
                    ["debtsILent"] = new
                    {
                        title = "Deudas que Presté",
                        description = "Dinero que otros me deben",
                        data = debtsILent.Data,
                        totalAmount = debtsILent.Data?.Items?.Sum(d => d.Amount) ?? 0
                    },
                    ["debtsIOwn"] = new
                    {
                        title = "Deudas que Debo",
                        description = "Dinero que debo a otros",
                        data = debtsIOwn.Data,
                        totalAmount = debtsIOwn.Data?.Items?.Sum(d => d.Amount) ?? 0
                    },
                    ["summary"] = new
                    {
                        totalLent = debtsILent.Data?.Items?.Sum(d => d.Amount) ?? 0,
                        totalOwned = debtsIOwn.Data?.Items?.Sum(d => d.Amount) ?? 0,
                        netBalance = (debtsILent.Data?.Items?.Sum(d => d.Amount) ?? 0) - (debtsIOwn.Data?.Items?.Sum(d => d.Amount) ?? 0),
                        lastUpdated = DateTime.UtcNow
                    }
                };

                return ApiResponse<Dictionary<string, object>>.SuccessResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las deudas para usuario: {UserId}", userId);
                return ApiResponse<Dictionary<string, object>>.ErrorResult("Error interno del servidor");
            }
        }

        // Métodos helper privados
        private static DebtDto MapToDebtDto(Debt debt, User owner, User? debtor)
        {
            return new DebtDto
            {
                Id = debt.Id,
                UserId = debt.UserId,
                DebtorId = debt.DebtorId,
                Title = debt.Title,
                Description = debt.Description,
                Amount = debt.Amount,
                Currency = debt.Currency ?? "COP",
                IsPaid = debt.IsPaid ?? false,
                DueDate = debt.DueDate,
                PaidAt = debt.PaidAt,
                CreatedAt = debt.CreatedAt ?? DateTime.UtcNow,
                UpdatedAt = debt.UpdatedAt ?? DateTime.UtcNow,
                OwnerName = owner != null ? $"{owner.FirstName} {owner.LastName}" : "",
                OwnerEmail = owner?.Email ?? "",
                DebtorName = debtor != null ? $"{debtor.FirstName} {debtor.LastName}" : null,
                DebtorEmail = debtor?.Email
            };
        }

        private static IQueryable<Debt> ApplyOrdering(IQueryable<Debt> query, string orderBy, string orderDirection)
        {
            var ascending = orderDirection.ToLower() == "asc";

            return orderBy.ToLower() switch
            {
                "amount" => ascending ? query.OrderBy(d => d.Amount) : query.OrderByDescending(d => d.Amount),
                "duedate" => ascending ? query.OrderBy(d => d.DueDate) : query.OrderByDescending(d => d.DueDate),
                "title" => ascending ? query.OrderBy(d => d.Title) : query.OrderByDescending(d => d.Title),
                "ispaid" => ascending ? query.OrderBy(d => d.IsPaid) : query.OrderByDescending(d => d.IsPaid),
                "updatedat" => ascending ? query.OrderBy(d => d.UpdatedAt) : query.OrderByDescending(d => d.UpdatedAt),
                _ => ascending ? query.OrderBy(d => d.CreatedAt) : query.OrderByDescending(d => d.CreatedAt)
            };
        }

        private static byte[] GenerateCsv(List<DebtDto> debts)
        {
            var csv = new StringBuilder();

            // Header
            csv.AppendLine("Id,Título,Descripción,Monto,Moneda,Estado,Deudor,Fecha Vencimiento,Fecha Pago,Fecha Creación");

            // Data
            foreach (var debt in debts)
            {
                csv.AppendLine($"{debt.Id}," +
                              $"\"{debt.Title}\"," +
                              $"\"{debt.Description ?? ""}\"," +
                              $"{debt.Amount}," +
                              $"{debt.Currency}," +
                              $"{debt.Status}," +
                              $"\"{debt.DebtorName ?? ""}\"," +
                              $"{debt.DueDate:yyyy-MM-dd HH:mm:ss}," +
                              $"{debt.PaidAt:yyyy-MM-dd HH:mm:ss}," +
                              $"{debt.CreatedAt:yyyy-MM-dd HH:mm:ss}");
            }

            return Encoding.UTF8.GetBytes(csv.ToString());
        }

        private async Task InvalidateUserCache(int userId)
        {
            try
            {
                await _cache.DeleteAsync($"user_debt_stats_{userId}");
                await _cache.DeleteAsync($"user_debts_{userId}");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error al invalidar caché de usuario: {UserId}", userId);
            }
        }

        private async Task InvalidateDebtCache(int debtId)
        {
            try
            {
                await _cache.DeleteAsync($"debt_{debtId}");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error al invalidar caché de deuda: {DebtId}", debtId);
            }
        }
    }
}