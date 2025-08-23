using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DebtCheckerBackend.DTO
{
    public class DebtDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int? DebtorId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "COP";
        public bool IsPaid { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? PaidAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Información del propietario de la deuda
        public string OwnerName { get; set; } = string.Empty;
        public string OwnerEmail { get; set; } = string.Empty;

        // Información del deudor 
        public string? DebtorName { get; set; }
        public string? DebtorEmail { get; set; }

        // Campos calculados
        public string Status => IsPaid ? "Pagada" : "Pendiente";
        public bool IsOverdue => !IsPaid && DueDate.HasValue && DueDate.Value < DateTime.UtcNow;
        public int? DaysUntilDue => DueDate.HasValue ? (DueDate.Value.Date - DateTime.UtcNow.Date).Days : null;
        public string FormattedAmount => $"{Amount:N2} {Currency}";
    }
}
