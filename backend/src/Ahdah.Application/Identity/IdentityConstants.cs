namespace Ahdah.Application.Identity;

public static class IdentityConstants
{
    public const string ManagerRole = "Manager";
    public const string ActiveStatus = "Active";
    public const string VerifiedIdentityStatus = "Verified";
}

public static class AhdahClaimTypes
{
    public const string CompanyId = "company_id";
    public const string Role = "role";
    public const string Name = "name";
}

public static class AhdahAuthorizationPolicies
{
    public const string AuthenticatedUser = "AuthenticatedUser";
    public const string CompanyMember = "CompanyMember";
    public const string CompanyDirectoryViewer = "CompanyDirectoryViewer";
    public const string ProjectViewer = "ProjectViewer";
    public const string AdvanceViewer = "AdvanceViewer";
    public const string AdvanceCreator = "AdvanceCreator";
    public const string AdvanceDistributor = "AdvanceDistributor";
    public const string AdvanceParticipant = "AdvanceParticipant";
    public const string AdvanceBalanceViewer = "AdvanceBalanceViewer";
    public const string ExpenseViewer = "ExpenseViewer";
    public const string ExpenseCreator = "ExpenseCreator";
    public const string ExpenseReviewer = "ExpenseReviewer";
    public const string ExpenseDocumentContributor = "ExpenseDocumentContributor";
    public const string ExpenseCategoryManager = "ExpenseCategoryManager";
    public const string ReimbursementViewer = "ReimbursementViewer";
    public const string ManagerOnly = "ManagerOnly";
}
