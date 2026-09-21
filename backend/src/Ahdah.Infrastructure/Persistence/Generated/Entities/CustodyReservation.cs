using System;
using System.Collections.Generic;

namespace Ahdah.Infrastructure.Persistence.Generated.Entities;

public partial class CustodyReservation
{
    public Guid CustodyReservationId { get; set; }

    public Guid CompanyId { get; set; }

    public Guid CustodyOperationId { get; set; }

    public Guid ProjectCustodyPositionId { get; set; }

    public Guid? ExpenseAdvanceAllocationId { get; set; }

    public Guid? TransferAdvanceAllocationId { get; set; }

    public decimal Amount { get; set; }

    public string Status { get; set; } = null!;

    public Guid? ResolvedByOperationId { get; set; }

    public int VersionNumber { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual Company Company { get; set; } = null!;

    public virtual CustodyOperation CustodyOperation { get; set; } = null!;

    public virtual CustodyOperation? CustodyOperationNavigation { get; set; }

    public virtual ExpenseAdvanceAllocation? ExpenseAdvanceAllocation { get; set; }

    public virtual ProjectCustodyPosition ProjectCustodyPosition { get; set; } = null!;

    public virtual TransferAdvanceAllocation? TransferAdvanceAllocation { get; set; }
}
