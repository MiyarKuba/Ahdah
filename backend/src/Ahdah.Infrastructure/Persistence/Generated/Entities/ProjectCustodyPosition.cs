using System;
using System.Collections.Generic;

namespace Ahdah.Infrastructure.Persistence.Generated.Entities;

public partial class ProjectCustodyPosition
{
    public Guid ProjectCustodyPositionId { get; set; }

    public Guid CompanyId { get; set; }

    public Guid UserAdvanceBalanceId { get; set; }

    public Guid? ProjectId { get; set; }

    public decimal AvailableAmount { get; set; }

    public decimal ReservedAmount { get; set; }

    public int VersionNumber { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual Company Company { get; set; } = null!;

    public virtual ICollection<CustodyMovementEntry> CustodyMovementEntries { get; set; } = new List<CustodyMovementEntry>();

    public virtual ICollection<CustodyReservation> CustodyReservations { get; set; } = new List<CustodyReservation>();

    public virtual Project? Project { get; set; }

    public virtual UserAdvanceBalance UserAdvanceBalance { get; set; } = null!;
}
