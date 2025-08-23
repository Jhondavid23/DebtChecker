using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DebtCheckerBackend.DTO
{
    public class DebtFilters
    {
        public bool? IsPaid { get; set; }
        public int? DebtorId { get; set; }
        public decimal? MinAmount { get; set; }
        public decimal? MaxAmount { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? Currency { get; set; }
        public bool? IsOverdue { get; set; }
        public string? Search { get; set; } // Para buscar en título y descripción
        public string OrderBy { get; set; } = "CreatedAt"; // CreatedAt, Amount, DueDate, Title
        public string OrderDirection { get; set; } = "desc"; // asc, desc
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
