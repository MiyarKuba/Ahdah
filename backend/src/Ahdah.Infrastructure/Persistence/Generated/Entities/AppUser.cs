using System;
using System.Collections.Generic;

namespace Ahdah.Infrastructure.Persistence.Generated.Entities;

public partial class AppUser
{
    public Guid UserId { get; set; }

    public Guid CompanyId { get; set; }

    public string FullName { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string? Email { get; set; }

    public string PasswordHash { get; set; } = null!;

    public string? TransactionPinHash { get; set; }

    public string Role { get; set; } = null!;

    public string Status { get; set; } = null!;

    public string IdentityVerificationStatus { get; set; } = null!;

    public DateTime? PhoneVerifiedAt { get; set; }

    public DateTime? IdentityVerifiedAt { get; set; }

    public Guid? ApprovedByUserId { get; set; }

    public DateTime? ApprovedAt { get; set; }

    public int FailedLoginAttempts { get; set; }

    public DateTime? LockedUntil { get; set; }

    public bool MustChangePassword { get; set; }

    public DateTime? LastLoginAt { get; set; }

    public DateTime? DeactivatedAt { get; set; }

    public string? StatusReason { get; set; }

    public int VersionNumber { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<Advance> AdvanceAppUser1s { get; set; } = new List<Advance>();

    public virtual ICollection<Advance> AdvanceAppUser2s { get; set; } = new List<Advance>();

    public virtual ICollection<Advance> AdvanceAppUser3s { get; set; } = new List<Advance>();

    public virtual ICollection<Advance> AdvanceAppUserNavigations { get; set; } = new List<Advance>();

    public virtual ICollection<Advance> AdvanceAppUsers { get; set; } = new List<Advance>();

    public virtual ICollection<AdvanceClosure> AdvanceClosureAppUser1s { get; set; } = new List<AdvanceClosure>();

    public virtual ICollection<AdvanceClosure> AdvanceClosureAppUser2s { get; set; } = new List<AdvanceClosure>();

    public virtual ICollection<AdvanceClosure> AdvanceClosureAppUser3s { get; set; } = new List<AdvanceClosure>();

    public virtual ICollection<AdvanceClosure> AdvanceClosureAppUser4s { get; set; } = new List<AdvanceClosure>();

    public virtual ICollection<AdvanceClosure> AdvanceClosureAppUserNavigations { get; set; } = new List<AdvanceClosure>();

    public virtual ICollection<AdvanceClosure> AdvanceClosureAppUsers { get; set; } = new List<AdvanceClosure>();

    public virtual ICollection<AdvanceClosureDocument> AdvanceClosureDocumentAppUserNavigations { get; set; } = new List<AdvanceClosureDocument>();

    public virtual ICollection<AdvanceClosureDocument> AdvanceClosureDocumentAppUsers { get; set; } = new List<AdvanceClosureDocument>();

    public virtual ICollection<AdvanceFundingSource> AdvanceFundingSources { get; set; } = new List<AdvanceFundingSource>();

    public virtual ICollection<AdvanceSettlement> AdvanceSettlementAppUser1s { get; set; } = new List<AdvanceSettlement>();

    public virtual ICollection<AdvanceSettlement> AdvanceSettlementAppUser2s { get; set; } = new List<AdvanceSettlement>();

    public virtual ICollection<AdvanceSettlement> AdvanceSettlementAppUser3s { get; set; } = new List<AdvanceSettlement>();

    public virtual ICollection<AdvanceSettlement> AdvanceSettlementAppUserNavigations { get; set; } = new List<AdvanceSettlement>();

    public virtual ICollection<AdvanceSettlement> AdvanceSettlementAppUsers { get; set; } = new List<AdvanceSettlement>();

    public virtual ICollection<AdvanceSettlementDocument> AdvanceSettlementDocumentAppUserNavigations { get; set; } = new List<AdvanceSettlementDocument>();

    public virtual ICollection<AdvanceSettlementDocument> AdvanceSettlementDocumentAppUsers { get; set; } = new List<AdvanceSettlementDocument>();

    public virtual ICollection<AdvanceSettlementResolution> AdvanceSettlementResolutionAppUser1s { get; set; } = new List<AdvanceSettlementResolution>();

    public virtual ICollection<AdvanceSettlementResolution> AdvanceSettlementResolutionAppUser2s { get; set; } = new List<AdvanceSettlementResolution>();

    public virtual ICollection<AdvanceSettlementResolution> AdvanceSettlementResolutionAppUser3s { get; set; } = new List<AdvanceSettlementResolution>();

    public virtual ICollection<AdvanceSettlementResolution> AdvanceSettlementResolutionAppUser4s { get; set; } = new List<AdvanceSettlementResolution>();

    public virtual ICollection<AdvanceSettlementResolution> AdvanceSettlementResolutionAppUserNavigations { get; set; } = new List<AdvanceSettlementResolution>();

    public virtual ICollection<AdvanceSettlementResolution> AdvanceSettlementResolutionAppUsers { get; set; } = new List<AdvanceSettlementResolution>();

    public virtual AppUser? AppUserNavigation { get; set; }

    public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();

    public virtual ICollection<BackgroundJob> BackgroundJobAppUserNavigations { get; set; } = new List<BackgroundJob>();

    public virtual ICollection<BackgroundJob> BackgroundJobAppUsers { get; set; } = new List<BackgroundJob>();

    public virtual ICollection<BalanceLedgerEntry> BalanceLedgerEntries { get; set; } = new List<BalanceLedgerEntry>();

    public virtual Company Company { get; set; } = null!;

    public virtual ICollection<CompanyCashboxEntry> CompanyCashboxEntryAppUser1s { get; set; } = new List<CompanyCashboxEntry>();

    public virtual ICollection<CompanyCashboxEntry> CompanyCashboxEntryAppUser2s { get; set; } = new List<CompanyCashboxEntry>();

    public virtual ICollection<CompanyCashboxEntry> CompanyCashboxEntryAppUserNavigations { get; set; } = new List<CompanyCashboxEntry>();

    public virtual ICollection<CompanyCashboxEntry> CompanyCashboxEntryAppUsers { get; set; } = new List<CompanyCashboxEntry>();

    public virtual ICollection<CompanyNumberSequenceAllocation> CompanyNumberSequenceAllocationAppUser1s { get; set; } = new List<CompanyNumberSequenceAllocation>();

    public virtual ICollection<CompanyNumberSequenceAllocation> CompanyNumberSequenceAllocationAppUserNavigations { get; set; } = new List<CompanyNumberSequenceAllocation>();

    public virtual ICollection<CompanyNumberSequenceAllocation> CompanyNumberSequenceAllocationAppUsers { get; set; } = new List<CompanyNumberSequenceAllocation>();

    public virtual ICollection<CompanyNumberSequence> CompanyNumberSequenceAppUser1s { get; set; } = new List<CompanyNumberSequence>();

    public virtual ICollection<CompanyNumberSequence> CompanyNumberSequenceAppUser2s { get; set; } = new List<CompanyNumberSequence>();

    public virtual ICollection<CompanyNumberSequence> CompanyNumberSequenceAppUser3s { get; set; } = new List<CompanyNumberSequence>();

    public virtual ICollection<CompanyNumberSequence> CompanyNumberSequenceAppUser4s { get; set; } = new List<CompanyNumberSequence>();

    public virtual ICollection<CompanyNumberSequence> CompanyNumberSequenceAppUserNavigations { get; set; } = new List<CompanyNumberSequence>();

    public virtual ICollection<CompanyNumberSequence> CompanyNumberSequenceAppUsers { get; set; } = new List<CompanyNumberSequence>();

    public virtual ICollection<CompanyNumberSequenceVersion> CompanyNumberSequenceVersionAppUserNavigations { get; set; } = new List<CompanyNumberSequenceVersion>();

    public virtual ICollection<CompanyNumberSequenceVersion> CompanyNumberSequenceVersionAppUsers { get; set; } = new List<CompanyNumberSequenceVersion>();

    public virtual ICollection<CompanySetting> CompanySettingAppUserNavigations { get; set; } = new List<CompanySetting>();

    public virtual ICollection<CompanySetting> CompanySettingAppUsers { get; set; } = new List<CompanySetting>();

    public virtual ICollection<ExpenseAdvanceAllocation> ExpenseAdvanceAllocations { get; set; } = new List<ExpenseAdvanceAllocation>();

    public virtual ICollection<Expense> ExpenseAppUser1s { get; set; } = new List<Expense>();

    public virtual ICollection<Expense> ExpenseAppUser2s { get; set; } = new List<Expense>();

    public virtual ICollection<Expense> ExpenseAppUser3s { get; set; } = new List<Expense>();

    public virtual ICollection<Expense> ExpenseAppUserNavigations { get; set; } = new List<Expense>();

    public virtual ICollection<Expense> ExpenseAppUsers { get; set; } = new List<Expense>();

    public virtual ICollection<ExpenseCategory> ExpenseCategoryAppUserNavigations { get; set; } = new List<ExpenseCategory>();

    public virtual ICollection<ExpenseCategory> ExpenseCategoryAppUsers { get; set; } = new List<ExpenseCategory>();

    public virtual ICollection<ExpenseDocument> ExpenseDocumentAppUserNavigations { get; set; } = new List<ExpenseDocument>();

    public virtual ICollection<ExpenseDocument> ExpenseDocumentAppUsers { get; set; } = new List<ExpenseDocument>();

    public virtual ICollection<ExpenseItem> ExpenseItems { get; set; } = new List<ExpenseItem>();

    public virtual ICollection<ExpenseReturnAdvanceAllocation> ExpenseReturnAdvanceAllocations { get; set; } = new List<ExpenseReturnAdvanceAllocation>();

    public virtual ICollection<ExpenseReturn> ExpenseReturnAppUser1s { get; set; } = new List<ExpenseReturn>();

    public virtual ICollection<ExpenseReturn> ExpenseReturnAppUser2s { get; set; } = new List<ExpenseReturn>();

    public virtual ICollection<ExpenseReturn> ExpenseReturnAppUser3s { get; set; } = new List<ExpenseReturn>();

    public virtual ICollection<ExpenseReturn> ExpenseReturnAppUser4s { get; set; } = new List<ExpenseReturn>();

    public virtual ICollection<ExpenseReturn> ExpenseReturnAppUserNavigations { get; set; } = new List<ExpenseReturn>();

    public virtual ICollection<ExpenseReturn> ExpenseReturnAppUsers { get; set; } = new List<ExpenseReturn>();

    public virtual ICollection<ExpenseReturnItem> ExpenseReturnItems { get; set; } = new List<ExpenseReturnItem>();

    public virtual ICollection<FundingSource> FundingSourceAppUser1s { get; set; } = new List<FundingSource>();

    public virtual ICollection<FundingSource> FundingSourceAppUser2s { get; set; } = new List<FundingSource>();

    public virtual ICollection<FundingSource> FundingSourceAppUserNavigations { get; set; } = new List<FundingSource>();

    public virtual ICollection<FundingSource> FundingSourceAppUsers { get; set; } = new List<FundingSource>();

    public virtual ICollection<FundingSourceLedgerEntry> FundingSourceLedgerEntries { get; set; } = new List<FundingSourceLedgerEntry>();

    public virtual ICollection<FundingSourcePaymentMethod> FundingSourcePaymentMethods { get; set; } = new List<FundingSourcePaymentMethod>();

    public virtual ICollection<IdempotencyRecord> IdempotencyRecords { get; set; } = new List<IdempotencyRecord>();

    public virtual ICollection<AppUser> InverseAppUserNavigation { get; set; } = new List<AppUser>();

    public virtual ICollection<Invitation> InvitationAppUserNavigations { get; set; } = new List<Invitation>();

    public virtual ICollection<Invitation> InvitationAppUsers { get; set; } = new List<Invitation>();

    public virtual JoinRequest? JoinRequestAppUserNavigation { get; set; }

    public virtual ICollection<JoinRequest> JoinRequestAppUsers { get; set; } = new List<JoinRequest>();

    public virtual ICollection<ManagerContribution> ManagerContributionAppUser1s { get; set; } = new List<ManagerContribution>();

    public virtual ICollection<ManagerContribution> ManagerContributionAppUser2s { get; set; } = new List<ManagerContribution>();

    public virtual ICollection<ManagerContribution> ManagerContributionAppUser3s { get; set; } = new List<ManagerContribution>();

    public virtual ICollection<ManagerContribution> ManagerContributionAppUserNavigations { get; set; } = new List<ManagerContribution>();

    public virtual ICollection<ManagerContribution> ManagerContributionAppUsers { get; set; } = new List<ManagerContribution>();

    public virtual ICollection<MoneyTransfer> MoneyTransferAppUser1s { get; set; } = new List<MoneyTransfer>();

    public virtual ICollection<MoneyTransfer> MoneyTransferAppUser2s { get; set; } = new List<MoneyTransfer>();

    public virtual ICollection<MoneyTransfer> MoneyTransferAppUser3s { get; set; } = new List<MoneyTransfer>();

    public virtual ICollection<MoneyTransfer> MoneyTransferAppUser4s { get; set; } = new List<MoneyTransfer>();

    public virtual ICollection<MoneyTransfer> MoneyTransferAppUser5s { get; set; } = new List<MoneyTransfer>();

    public virtual ICollection<MoneyTransfer> MoneyTransferAppUserNavigations { get; set; } = new List<MoneyTransfer>();

    public virtual ICollection<MoneyTransfer> MoneyTransferAppUsers { get; set; } = new List<MoneyTransfer>();

    public virtual ICollection<Notification> NotificationAppUser1s { get; set; } = new List<Notification>();

    public virtual ICollection<Notification> NotificationAppUserNavigations { get; set; } = new List<Notification>();

    public virtual ICollection<Notification> NotificationAppUsers { get; set; } = new List<Notification>();

    public virtual ICollection<NotificationDelivery> NotificationDeliveryAppUserNavigations { get; set; } = new List<NotificationDelivery>();

    public virtual ICollection<NotificationDelivery> NotificationDeliveryAppUsers { get; set; } = new List<NotificationDelivery>();

    public virtual ICollection<NotificationPreference> NotificationPreferenceAppUserNavigations { get; set; } = new List<NotificationPreference>();

    public virtual ICollection<NotificationPreference> NotificationPreferenceAppUsers { get; set; } = new List<NotificationPreference>();

    public virtual ICollection<NotificationTemplate> NotificationTemplateAppUser1s { get; set; } = new List<NotificationTemplate>();

    public virtual ICollection<NotificationTemplate> NotificationTemplateAppUser2s { get; set; } = new List<NotificationTemplate>();

    public virtual ICollection<NotificationTemplate> NotificationTemplateAppUser3s { get; set; } = new List<NotificationTemplate>();

    public virtual ICollection<NotificationTemplate> NotificationTemplateAppUserNavigations { get; set; } = new List<NotificationTemplate>();

    public virtual ICollection<NotificationTemplate> NotificationTemplateAppUsers { get; set; } = new List<NotificationTemplate>();

    public virtual ICollection<NotificationTemplateVersion> NotificationTemplateVersions { get; set; } = new List<NotificationTemplateVersion>();

    public virtual ICollection<OutboxMessage> OutboxMessageAppUserNavigations { get; set; } = new List<OutboxMessage>();

    public virtual ICollection<OutboxMessage> OutboxMessageAppUsers { get; set; } = new List<OutboxMessage>();

    public virtual ICollection<OwnerPaymentProjectAllocation> OwnerPaymentProjectAllocations { get; set; } = new List<OwnerPaymentProjectAllocation>();

    public virtual ICollection<OwnerPaymentRefund> OwnerPaymentRefundAppUser1s { get; set; } = new List<OwnerPaymentRefund>();

    public virtual ICollection<OwnerPaymentRefund> OwnerPaymentRefundAppUser2s { get; set; } = new List<OwnerPaymentRefund>();

    public virtual ICollection<OwnerPaymentRefund> OwnerPaymentRefundAppUserNavigations { get; set; } = new List<OwnerPaymentRefund>();

    public virtual ICollection<OwnerPaymentRefund> OwnerPaymentRefundAppUsers { get; set; } = new List<OwnerPaymentRefund>();

    public virtual ICollection<OwnerRefundFundingSource> OwnerRefundFundingSources { get; set; } = new List<OwnerRefundFundingSource>();

    public virtual ICollection<PersonalClaimAdjustment> PersonalClaimAdjustmentAppUser1s { get; set; } = new List<PersonalClaimAdjustment>();

    public virtual ICollection<PersonalClaimAdjustment> PersonalClaimAdjustmentAppUser2s { get; set; } = new List<PersonalClaimAdjustment>();

    public virtual ICollection<PersonalClaimAdjustment> PersonalClaimAdjustmentAppUser3s { get; set; } = new List<PersonalClaimAdjustment>();

    public virtual ICollection<PersonalClaimAdjustment> PersonalClaimAdjustmentAppUser4s { get; set; } = new List<PersonalClaimAdjustment>();

    public virtual ICollection<PersonalClaimAdjustment> PersonalClaimAdjustmentAppUserNavigations { get; set; } = new List<PersonalClaimAdjustment>();

    public virtual ICollection<PersonalClaimAdjustment> PersonalClaimAdjustmentAppUsers { get; set; } = new List<PersonalClaimAdjustment>();

    public virtual ICollection<PersonalClaim> PersonalClaimAppUser1s { get; set; } = new List<PersonalClaim>();

    public virtual ICollection<PersonalClaim> PersonalClaimAppUser2s { get; set; } = new List<PersonalClaim>();

    public virtual ICollection<PersonalClaim> PersonalClaimAppUserNavigations { get; set; } = new List<PersonalClaim>();

    public virtual ICollection<PersonalClaim> PersonalClaimAppUsers { get; set; } = new List<PersonalClaim>();

    public virtual ICollection<PersonalClaimLedgerEntry> PersonalClaimLedgerEntries { get; set; } = new List<PersonalClaimLedgerEntry>();

    public virtual ICollection<PersonalClaimPaymentAllocation> PersonalClaimPaymentAllocations { get; set; } = new List<PersonalClaimPaymentAllocation>();

    public virtual ICollection<PersonalClaimPayment> PersonalClaimPaymentAppUser1s { get; set; } = new List<PersonalClaimPayment>();

    public virtual ICollection<PersonalClaimPayment> PersonalClaimPaymentAppUser2s { get; set; } = new List<PersonalClaimPayment>();

    public virtual ICollection<PersonalClaimPayment> PersonalClaimPaymentAppUser3s { get; set; } = new List<PersonalClaimPayment>();

    public virtual ICollection<PersonalClaimPayment> PersonalClaimPaymentAppUser4s { get; set; } = new List<PersonalClaimPayment>();

    public virtual ICollection<PersonalClaimPayment> PersonalClaimPaymentAppUser5s { get; set; } = new List<PersonalClaimPayment>();

    public virtual ICollection<PersonalClaimPayment> PersonalClaimPaymentAppUserNavigations { get; set; } = new List<PersonalClaimPayment>();

    public virtual ICollection<PersonalClaimPayment> PersonalClaimPaymentAppUsers { get; set; } = new List<PersonalClaimPayment>();

    public virtual ICollection<PersonalClaimPaymentFundingSource> PersonalClaimPaymentFundingSources { get; set; } = new List<PersonalClaimPaymentFundingSource>();

    public virtual ICollection<PersonalClaimWriteOff> PersonalClaimWriteOffAppUser1s { get; set; } = new List<PersonalClaimWriteOff>();

    public virtual ICollection<PersonalClaimWriteOff> PersonalClaimWriteOffAppUser2s { get; set; } = new List<PersonalClaimWriteOff>();

    public virtual ICollection<PersonalClaimWriteOff> PersonalClaimWriteOffAppUser3s { get; set; } = new List<PersonalClaimWriteOff>();

    public virtual ICollection<PersonalClaimWriteOff> PersonalClaimWriteOffAppUser4s { get; set; } = new List<PersonalClaimWriteOff>();

    public virtual ICollection<PersonalClaimWriteOff> PersonalClaimWriteOffAppUserNavigations { get; set; } = new List<PersonalClaimWriteOff>();

    public virtual ICollection<PersonalClaimWriteOff> PersonalClaimWriteOffAppUsers { get; set; } = new List<PersonalClaimWriteOff>();

    public virtual ICollection<Project> ProjectAppUserNavigations { get; set; } = new List<Project>();

    public virtual ICollection<Project> ProjectAppUsers { get; set; } = new List<Project>();

    public virtual ICollection<ProjectContractChange> ProjectContractChangeAppUser1s { get; set; } = new List<ProjectContractChange>();

    public virtual ICollection<ProjectContractChange> ProjectContractChangeAppUserNavigations { get; set; } = new List<ProjectContractChange>();

    public virtual ICollection<ProjectContractChange> ProjectContractChangeAppUsers { get; set; } = new List<ProjectContractChange>();

    public virtual ICollection<ProjectOwnerPayment> ProjectOwnerPaymentAppUser1s { get; set; } = new List<ProjectOwnerPayment>();

    public virtual ICollection<ProjectOwnerPayment> ProjectOwnerPaymentAppUser2s { get; set; } = new List<ProjectOwnerPayment>();

    public virtual ICollection<ProjectOwnerPayment> ProjectOwnerPaymentAppUserNavigations { get; set; } = new List<ProjectOwnerPayment>();

    public virtual ICollection<ProjectOwnerPayment> ProjectOwnerPaymentAppUsers { get; set; } = new List<ProjectOwnerPayment>();

    public virtual ICollection<ProjectOwner> ProjectOwners { get; set; } = new List<ProjectOwner>();

    public virtual ICollection<ProjectSupervisor> ProjectSupervisorAppUser1s { get; set; } = new List<ProjectSupervisor>();

    public virtual ICollection<ProjectSupervisor> ProjectSupervisorAppUserNavigations { get; set; } = new List<ProjectSupervisor>();

    public virtual ICollection<ProjectSupervisor> ProjectSupervisorAppUsers { get; set; } = new List<ProjectSupervisor>();

    public virtual ICollection<ReportDefinition> ReportDefinitionAppUser1s { get; set; } = new List<ReportDefinition>();

    public virtual ICollection<ReportDefinition> ReportDefinitionAppUser2s { get; set; } = new List<ReportDefinition>();

    public virtual ICollection<ReportDefinition> ReportDefinitionAppUser3s { get; set; } = new List<ReportDefinition>();

    public virtual ICollection<ReportDefinition> ReportDefinitionAppUserNavigations { get; set; } = new List<ReportDefinition>();

    public virtual ICollection<ReportDefinition> ReportDefinitionAppUsers { get; set; } = new List<ReportDefinition>();

    public virtual ICollection<ReportDefinitionVersion> ReportDefinitionVersions { get; set; } = new List<ReportDefinitionVersion>();

    public virtual ICollection<ReportDelivery> ReportDeliveryAppUserNavigations { get; set; } = new List<ReportDelivery>();

    public virtual ICollection<ReportDelivery> ReportDeliveryAppUsers { get; set; } = new List<ReportDelivery>();

    public virtual ICollection<ReportDeliveryAttempt> ReportDeliveryAttempts { get; set; } = new List<ReportDeliveryAttempt>();

    public virtual ICollection<ReportDeliveryEvent> ReportDeliveryEvents { get; set; } = new List<ReportDeliveryEvent>();

    public virtual ICollection<ReportDeliveryFile> ReportDeliveryFiles { get; set; } = new List<ReportDeliveryFile>();

    public virtual ICollection<ReportFileDownload> ReportFileDownloads { get; set; } = new List<ReportFileDownload>();

    public virtual ICollection<ReportRecipientSuppression> ReportRecipientSuppressionAppUser1s { get; set; } = new List<ReportRecipientSuppression>();

    public virtual ICollection<ReportRecipientSuppression> ReportRecipientSuppressionAppUserNavigations { get; set; } = new List<ReportRecipientSuppression>();

    public virtual ICollection<ReportRecipientSuppression> ReportRecipientSuppressionAppUsers { get; set; } = new List<ReportRecipientSuppression>();

    public virtual ICollection<ReportRecipientSuppressionEvent> ReportRecipientSuppressionEventAppUser1s { get; set; } = new List<ReportRecipientSuppressionEvent>();

    public virtual ICollection<ReportRecipientSuppressionEvent> ReportRecipientSuppressionEventAppUserNavigations { get; set; } = new List<ReportRecipientSuppressionEvent>();

    public virtual ICollection<ReportRecipientSuppressionEvent> ReportRecipientSuppressionEventAppUsers { get; set; } = new List<ReportRecipientSuppressionEvent>();

    public virtual ICollection<ReportRun> ReportRunAppUserNavigations { get; set; } = new List<ReportRun>();

    public virtual ICollection<ReportRun> ReportRunAppUsers { get; set; } = new List<ReportRun>();

    public virtual ICollection<ReportRunFile> ReportRunFileAppUserNavigations { get; set; } = new List<ReportRunFile>();

    public virtual ICollection<ReportRunFile> ReportRunFileAppUsers { get; set; } = new List<ReportRunFile>();

    public virtual ICollection<ReportSchedule> ReportScheduleAppUser1s { get; set; } = new List<ReportSchedule>();

    public virtual ICollection<ReportSchedule> ReportScheduleAppUser2s { get; set; } = new List<ReportSchedule>();

    public virtual ICollection<ReportSchedule> ReportScheduleAppUser3s { get; set; } = new List<ReportSchedule>();

    public virtual ICollection<ReportSchedule> ReportScheduleAppUser4s { get; set; } = new List<ReportSchedule>();

    public virtual ICollection<ReportSchedule> ReportScheduleAppUser5s { get; set; } = new List<ReportSchedule>();

    public virtual ICollection<ReportSchedule> ReportScheduleAppUserNavigations { get; set; } = new List<ReportSchedule>();

    public virtual ICollection<ReportSchedule> ReportScheduleAppUsers { get; set; } = new List<ReportSchedule>();

    public virtual ICollection<ReportScheduleRecipient> ReportScheduleRecipientAppUser1s { get; set; } = new List<ReportScheduleRecipient>();

    public virtual ICollection<ReportScheduleRecipient> ReportScheduleRecipientAppUser2s { get; set; } = new List<ReportScheduleRecipient>();

    public virtual ICollection<ReportScheduleRecipient> ReportScheduleRecipientAppUser3s { get; set; } = new List<ReportScheduleRecipient>();

    public virtual ICollection<ReportScheduleRecipient> ReportScheduleRecipientAppUser4s { get; set; } = new List<ReportScheduleRecipient>();

    public virtual ICollection<ReportScheduleRecipient> ReportScheduleRecipientAppUserNavigations { get; set; } = new List<ReportScheduleRecipient>();

    public virtual ICollection<ReportScheduleRecipient> ReportScheduleRecipientAppUsers { get; set; } = new List<ReportScheduleRecipient>();

    public virtual ICollection<SavedReportView> SavedReportViewAppUser1s { get; set; } = new List<SavedReportView>();

    public virtual ICollection<SavedReportView> SavedReportViewAppUser2s { get; set; } = new List<SavedReportView>();

    public virtual ICollection<SavedReportView> SavedReportViewAppUser3s { get; set; } = new List<SavedReportView>();

    public virtual ICollection<SavedReportView> SavedReportViewAppUserNavigations { get; set; } = new List<SavedReportView>();

    public virtual ICollection<SavedReportView> SavedReportViewAppUsers { get; set; } = new List<SavedReportView>();

    public virtual ICollection<SavedReportViewShare> SavedReportViewShareAppUser1s { get; set; } = new List<SavedReportViewShare>();

    public virtual ICollection<SavedReportViewShare> SavedReportViewShareAppUserNavigations { get; set; } = new List<SavedReportViewShare>();

    public virtual ICollection<SavedReportViewShare> SavedReportViewShareAppUsers { get; set; } = new List<SavedReportViewShare>();

    public virtual ICollection<SavedReportViewShareEvent> SavedReportViewShareEventAppUser1s { get; set; } = new List<SavedReportViewShareEvent>();

    public virtual ICollection<SavedReportViewShareEvent> SavedReportViewShareEventAppUser2s { get; set; } = new List<SavedReportViewShareEvent>();

    public virtual ICollection<SavedReportViewShareEvent> SavedReportViewShareEventAppUserNavigations { get; set; } = new List<SavedReportViewShareEvent>();

    public virtual ICollection<SavedReportViewShareEvent> SavedReportViewShareEventAppUsers { get; set; } = new List<SavedReportViewShareEvent>();

    public virtual ICollection<SavedReportViewVersion> SavedReportViewVersionAppUser1s { get; set; } = new List<SavedReportViewVersion>();

    public virtual ICollection<SavedReportViewVersion> SavedReportViewVersionAppUser2s { get; set; } = new List<SavedReportViewVersion>();

    public virtual ICollection<SavedReportViewVersion> SavedReportViewVersionAppUser3s { get; set; } = new List<SavedReportViewVersion>();

    public virtual ICollection<SavedReportViewVersion> SavedReportViewVersionAppUser4s { get; set; } = new List<SavedReportViewVersion>();

    public virtual ICollection<SavedReportViewVersion> SavedReportViewVersionAppUserNavigations { get; set; } = new List<SavedReportViewVersion>();

    public virtual ICollection<SavedReportViewVersion> SavedReportViewVersionAppUsers { get; set; } = new List<SavedReportViewVersion>();

    public virtual ICollection<ScheduledJobDefinition> ScheduledJobDefinitionAppUser1s { get; set; } = new List<ScheduledJobDefinition>();

    public virtual ICollection<ScheduledJobDefinition> ScheduledJobDefinitionAppUser2s { get; set; } = new List<ScheduledJobDefinition>();

    public virtual ICollection<ScheduledJobDefinition> ScheduledJobDefinitionAppUser3s { get; set; } = new List<ScheduledJobDefinition>();

    public virtual ICollection<ScheduledJobDefinition> ScheduledJobDefinitionAppUser4s { get; set; } = new List<ScheduledJobDefinition>();

    public virtual ICollection<ScheduledJobDefinition> ScheduledJobDefinitionAppUserNavigations { get; set; } = new List<ScheduledJobDefinition>();

    public virtual ICollection<ScheduledJobDefinition> ScheduledJobDefinitionAppUsers { get; set; } = new List<ScheduledJobDefinition>();

    public virtual ICollection<Supplier> SupplierAppUserNavigations { get; set; } = new List<Supplier>();

    public virtual ICollection<Supplier> SupplierAppUsers { get; set; } = new List<Supplier>();

    public virtual ICollection<SupplierCreditNoteAllocation> SupplierCreditNoteAllocations { get; set; } = new List<SupplierCreditNoteAllocation>();

    public virtual ICollection<SupplierCreditNote> SupplierCreditNoteAppUser1s { get; set; } = new List<SupplierCreditNote>();

    public virtual ICollection<SupplierCreditNote> SupplierCreditNoteAppUser2s { get; set; } = new List<SupplierCreditNote>();

    public virtual ICollection<SupplierCreditNote> SupplierCreditNoteAppUser3s { get; set; } = new List<SupplierCreditNote>();

    public virtual ICollection<SupplierCreditNote> SupplierCreditNoteAppUser4s { get; set; } = new List<SupplierCreditNote>();

    public virtual ICollection<SupplierCreditNote> SupplierCreditNoteAppUserNavigations { get; set; } = new List<SupplierCreditNote>();

    public virtual ICollection<SupplierCreditNote> SupplierCreditNoteAppUsers { get; set; } = new List<SupplierCreditNote>();

    public virtual ICollection<SupplierDebtAdjustment> SupplierDebtAdjustmentAppUser1s { get; set; } = new List<SupplierDebtAdjustment>();

    public virtual ICollection<SupplierDebtAdjustment> SupplierDebtAdjustmentAppUser2s { get; set; } = new List<SupplierDebtAdjustment>();

    public virtual ICollection<SupplierDebtAdjustment> SupplierDebtAdjustmentAppUser3s { get; set; } = new List<SupplierDebtAdjustment>();

    public virtual ICollection<SupplierDebtAdjustment> SupplierDebtAdjustmentAppUser4s { get; set; } = new List<SupplierDebtAdjustment>();

    public virtual ICollection<SupplierDebtAdjustment> SupplierDebtAdjustmentAppUserNavigations { get; set; } = new List<SupplierDebtAdjustment>();

    public virtual ICollection<SupplierDebtAdjustment> SupplierDebtAdjustmentAppUsers { get; set; } = new List<SupplierDebtAdjustment>();

    public virtual ICollection<SupplierDebt> SupplierDebtAppUser1s { get; set; } = new List<SupplierDebt>();

    public virtual ICollection<SupplierDebt> SupplierDebtAppUserNavigations { get; set; } = new List<SupplierDebt>();

    public virtual ICollection<SupplierDebt> SupplierDebtAppUsers { get; set; } = new List<SupplierDebt>();

    public virtual ICollection<SupplierDebtLedgerEntry> SupplierDebtLedgerEntries { get; set; } = new List<SupplierDebtLedgerEntry>();

    public virtual ICollection<SupplierDebtWriteOff> SupplierDebtWriteOffAppUser1s { get; set; } = new List<SupplierDebtWriteOff>();

    public virtual ICollection<SupplierDebtWriteOff> SupplierDebtWriteOffAppUser2s { get; set; } = new List<SupplierDebtWriteOff>();

    public virtual ICollection<SupplierDebtWriteOff> SupplierDebtWriteOffAppUser3s { get; set; } = new List<SupplierDebtWriteOff>();

    public virtual ICollection<SupplierDebtWriteOff> SupplierDebtWriteOffAppUser4s { get; set; } = new List<SupplierDebtWriteOff>();

    public virtual ICollection<SupplierDebtWriteOff> SupplierDebtWriteOffAppUserNavigations { get; set; } = new List<SupplierDebtWriteOff>();

    public virtual ICollection<SupplierDebtWriteOff> SupplierDebtWriteOffAppUsers { get; set; } = new List<SupplierDebtWriteOff>();

    public virtual ICollection<SupplierPaymentAccount> SupplierPaymentAccountAppUser1s { get; set; } = new List<SupplierPaymentAccount>();

    public virtual ICollection<SupplierPaymentAccount> SupplierPaymentAccountAppUserNavigations { get; set; } = new List<SupplierPaymentAccount>();

    public virtual ICollection<SupplierPaymentAccount> SupplierPaymentAccountAppUsers { get; set; } = new List<SupplierPaymentAccount>();

    public virtual ICollection<SupplierPayment> SupplierPaymentAppUser1s { get; set; } = new List<SupplierPayment>();

    public virtual ICollection<SupplierPayment> SupplierPaymentAppUser2s { get; set; } = new List<SupplierPayment>();

    public virtual ICollection<SupplierPayment> SupplierPaymentAppUser3s { get; set; } = new List<SupplierPayment>();

    public virtual ICollection<SupplierPayment> SupplierPaymentAppUser4s { get; set; } = new List<SupplierPayment>();

    public virtual ICollection<SupplierPayment> SupplierPaymentAppUserNavigations { get; set; } = new List<SupplierPayment>();

    public virtual ICollection<SupplierPayment> SupplierPaymentAppUsers { get; set; } = new List<SupplierPayment>();

    public virtual ICollection<SupplierPaymentDebtAllocation> SupplierPaymentDebtAllocations { get; set; } = new List<SupplierPaymentDebtAllocation>();

    public virtual ICollection<SupplierPaymentFundingSource> SupplierPaymentFundingSources { get; set; } = new List<SupplierPaymentFundingSource>();

    public virtual ICollection<SupplierRefund> SupplierRefundAppUser1s { get; set; } = new List<SupplierRefund>();

    public virtual ICollection<SupplierRefund> SupplierRefundAppUser2s { get; set; } = new List<SupplierRefund>();

    public virtual ICollection<SupplierRefund> SupplierRefundAppUserNavigations { get; set; } = new List<SupplierRefund>();

    public virtual ICollection<SupplierRefund> SupplierRefundAppUsers { get; set; } = new List<SupplierRefund>();

    public virtual ICollection<TransferAdvanceAllocation> TransferAdvanceAllocations { get; set; } = new List<TransferAdvanceAllocation>();

    public virtual ICollection<TransferCorrectionRequest> TransferCorrectionRequestAppUser1s { get; set; } = new List<TransferCorrectionRequest>();

    public virtual ICollection<TransferCorrectionRequest> TransferCorrectionRequestAppUser2s { get; set; } = new List<TransferCorrectionRequest>();

    public virtual ICollection<TransferCorrectionRequest> TransferCorrectionRequestAppUserNavigations { get; set; } = new List<TransferCorrectionRequest>();

    public virtual ICollection<TransferCorrectionRequest> TransferCorrectionRequestAppUsers { get; set; } = new List<TransferCorrectionRequest>();

    public virtual ICollection<UserAdvanceBalance> UserAdvanceBalanceAppUserNavigations { get; set; } = new List<UserAdvanceBalance>();

    public virtual ICollection<UserAdvanceBalance> UserAdvanceBalanceAppUsers { get; set; } = new List<UserAdvanceBalance>();

    public virtual ICollection<UserDevice> UserDevices { get; set; } = new List<UserDevice>();
}
