using System;
using System.Collections.Generic;

namespace DebtCheckerBackend.Model;

public partial class Debt
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int? DebtorId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Amount { get; set; }

    public string? Currency { get; set; }

    public bool? IsPaid { get; set; }

    public DateTime? DueDate { get; set; }

    public DateTime? PaidAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual User? Debtor { get; set; }

    public virtual User User { get; set; } = null!;
}
