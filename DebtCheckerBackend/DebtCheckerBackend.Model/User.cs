using System;
using System.Collections.Generic;

namespace DebtCheckerBackend.Model;

public partial class User
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Debt> DebtDebtors { get; set; } = new List<Debt>();

    public virtual ICollection<Debt> DebtUsers { get; set; } = new List<Debt>();
}
