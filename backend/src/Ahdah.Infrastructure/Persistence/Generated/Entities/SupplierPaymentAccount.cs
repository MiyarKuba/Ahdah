using System;
using System.Collections.Generic;

namespace Ahdah.Infrastructure.Persistence.Generated.Entities;

public partial class SupplierPaymentAccount
{
    public Guid SupplierPaymentAccountId { get; set; }

    public Guid CompanyId { get; set; }

    public Guid SupplierId { get; set; }

    public string AccountType { get; set; } = null!;

    public string AccountLabel { get; set; } = null!;

    public string? AccountHolderName { get; set; }

    public string? BankName { get; set; }

    public string? BankBranchName { get; set; }

    public string? AccountNumber { get; set; }

    public string? Iban { get; set; }

    public string? WalletProvider { get; set; }

    public string? WalletNumber { get; set; }

    public string CurrencyCode { get; set; } = null!;

    public bool IsDefault { get; set; }

    public string VerificationStatus { get; set; } = null!;

    public Guid? VerifiedByUserId { get; set; }

    public DateTime? VerifiedAt { get; set; }

    public string? RejectionReason { get; set; }

    public string? Notes { get; set; }

    public bool IsActive { get; set; }

    public Guid CreatedByUserId { get; set; }

    public Guid? DeactivatedByUserId { get; set; }

    public DateTime? DeactivatedAt { get; set; }

    public string? DeactivationReason { get; set; }

    public int VersionNumber { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual AppUser AppUser { get; set; } = null!;

    public virtual AppUser? AppUser1 { get; set; }

    public virtual AppUser? AppUserNavigation { get; set; }

    public virtual Supplier Supplier { get; set; } = null!;

    public virtual ICollection<SupplierPayment> SupplierPayments { get; set; } = new List<SupplierPayment>();
}
