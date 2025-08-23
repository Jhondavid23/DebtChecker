using DebtCheckerBackend.DTO;
using DebtCheckerBackend.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DebtCheckerBackend.BLL.Contrato
{
    public interface IDebtService
    {
        // CRUD básico
        Task<ApiResponse<DebtDto>> CreateDebtAsync(int userId, CreateDebtRequest request);
        Task<ApiResponse<DebtDto>> GetDebtByIdAsync(int debtId, int userId);
        Task<ApiResponse<DebtDto>> UpdateDebtAsync(int debtId, int userId, UpdateDebtRequest request);
        Task<ApiResponse<bool>> DeleteDebtAsync(int debtId, int userId);

        // Operaciones específicas
        Task<ApiResponse<DebtDto>> PayDebtAsync(int debtId, int userId, PayDebtRequest? request = null);
        Task<ApiResponse<DebtDto>> UnpayDebtAsync(int debtId, int userId);

        // Consultas
        Task<ApiResponse<PaginatedResult<DebtDto>>> GetUserDebtsAsync(int userId, DebtFilters filters);
        Task<ApiResponse<List<DebtDto>>> GetOverdueDebtsAsync(int userId);
        Task<ApiResponse<List<DebtDto>>> GetRecentDebtsAsync(int userId, int count = 5);

        // Consultas para deudores
        Task<ApiResponse<PaginatedResult<DebtDto>>> GetMyDebtsAsDebtorAsync(int userId, DebtFilters filters);
        Task<ApiResponse<Dictionary<string, object>>> GetAllMyDebtsAsync(int userId, DebtFilters filters);

        // Estadísticas y reportes
        Task<ApiResponse<DebtStatistics>> GetUserStatisticsAsync(int userId);
        Task<ApiResponse<Dictionary<string, object>>> GetUserSummaryAsync(int userId);

        // Exportación
        Task<ApiResponse<byte[]>> ExportUserDebtsAsync(int userId, string format = "json"); // json, csv

        // Búsqueda
        Task<ApiResponse<PaginatedResult<DebtDto>>> SearchDebtsAsync(int userId, string searchTerm, int page = 1, int pageSize = 10);
    }
}
