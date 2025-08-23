using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DebtCheckerBackend.DTO
{
    public class PayDebtRequest
    {
        public DateTime? PaidAt { get; set; } = DateTime.UtcNow;
        public string? Notes { get; set; }
    }
}
