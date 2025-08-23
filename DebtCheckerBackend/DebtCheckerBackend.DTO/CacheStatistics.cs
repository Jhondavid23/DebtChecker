using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DebtCheckerBackend.DTO
{
    public class CacheStatistics
    {
        public int TotalEntries { get; set; }
        public string TableName { get; set; } = string.Empty;
        public DateTime LastUpdated { get; set; }
        public string? Error { get; set; }
    }
}
