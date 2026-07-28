using System;
using System.Collections.Generic;

namespace Ahdah.Infrastructure.Persistence.Generated.Entities;

public partial class Supplier
{
    public Guid SupplierId { get; set; }

    public Guid CompanyId { get; set; }

    public string? SupplierCode { get; set; }

    public string SupplierName { get; set; } = null!;

    public string SupplierType { get; set; } = null!;

    public string? ContactPersonName { get; set; }

    public string? PhoneNumber { get; set; }

    public string? SecondaryPhoneNumber { get; set; }

    public string? Email { get; set; }

    public string? CommercialRegistrationNumber { get; set; }

    public string? TaxRegistrationNumber { get; set; }

    public string? Address { get; set; }

    public string? City { get; set; }

    public string DefaultCurrencyCode { get; set; } = null!;

    public string TransactionMode { get; set; } = null!;

    public int DefaultPaymentTermsDays { get; set; }

    public decimal? CreditLimit { get; set; }

    public string? PreferredPaymentMethod { get; set; }

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

    public virtual AppUser? AppUserNavigation { get; set; }

    public virtual Company Company { get; set; } = null!;

    public virtual ICollection<Expense> Expenses { get; set; } = new List<Expense>();

    public virtual ICollection<SupplierCreditNote> SupplierCreditNotes { get; set; } = new List<SupplierCreditNote>();

    public virtual ICollection<SupplierDebt> SupplierDebts { get; set; } = new List<SupplierDebt>();

    public virtual ICollection<SupplierPaymentAccount> SupplierPaymentAccounts { get; set; } = new List<SupplierPaymentAccount>();

    public virtual ICollection<SupplierPayment> SupplierPayments { get; set; } = new List<SupplierPayment>();

    public virtual ICollection<SupplierRefund> SupplierRefunds { get; set; } = new List<SupplierRefund>();
}
