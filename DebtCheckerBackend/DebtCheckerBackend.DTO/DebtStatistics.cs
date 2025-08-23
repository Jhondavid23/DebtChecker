using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DebtCheckerBackend.DTO
{
    public class DebtStatistics
    {
        public int TotalDebts { get; set; }
        public int PendingDebts { get; set; }
        public int PaidDebts { get; set; }
        public int OverdueDebts { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PendingAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal OverdueAmount { get; set; }
        public string Currency { get; set; } = "COP";
        public DateTime LastUpdated { get; set; }
    }
}
