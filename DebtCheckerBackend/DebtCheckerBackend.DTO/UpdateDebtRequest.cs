using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DebtCheckerBackend.DTO
{
    public class UpdateDebtRequest
    {
        [Required(ErrorMessage = "El título es requerido")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "El título debe tener entre 3 y 200 caracteres")]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "La descripción no puede exceder 1000 caracteres")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "El monto es requerido")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor a 0")]
        public decimal Amount { get; set; }

        [StringLength(3, ErrorMessage = "La moneda debe tener máximo 3 caracteres")]
        public string Currency { get; set; } = "COP";

        public int? DebtorId { get; set; }

        public DateTime? DueDate { get; set; }
    }
}
