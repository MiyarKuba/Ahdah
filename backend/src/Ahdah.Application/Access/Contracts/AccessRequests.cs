using System.ComponentModel.DataAnnotations;

namespace Ahdah.Application.Access.Contracts;

public sealed record CreateInvitationRequest
{
    public CreateInvitationRequest(string phoneNumber, string assignedRole)
    {
        PhoneNumber = phoneNumber;
        AssignedRole = assignedRole;
    }

    [Required]
    [StringLength(40)]
    [RegularExpression("^\\s*\\+[1-9][0-9]{7,14}\\s*$")]
    public string PhoneNumber { get; init; }

    [Required]
    [StringLength(30)]
    [RegularExpression("^\\s*(Deputy|Accountant|Supervisor|Worker)\\s*$")]
    public string AssignedRole { get; init; }
}

public sealed record AcceptInvitationRequest
{
    public AcceptInvitationRequest(string token, string fullName, string password, string? email)
    {
        Token = token;
        FullName = fullName;
        Password = password;
        Email = email;
    }

    [Required]
    [StringLength(256, MinimumLength = 32)]
    [RegularExpression("^[A-Za-z0-9_-]+$")]
    public string Token { get; init; }

    [Required]
    [StringLength(200, MinimumLength = 1)]
    [RegularExpression("^(?=.*\\S)[^\\r\\n]+$")]
    public string FullName { get; init; }

    [Required]
    [StringLength(128, MinimumLength = 12)]
    public string Password { get; init; }

    [EmailAddress]
    [StringLength(254)]
    public string? Email { get; init; }
}

public sealed record ApproveJoinRequestRequest
{
    public ApproveJoinRequestRequest(string assignedRole, string? reviewNotes)
    {
        AssignedRole = assignedRole;
        ReviewNotes = reviewNotes;
    }

    [Required]
    [StringLength(30)]
    [RegularExpression("^\\s*(Deputy|Accountant|Supervisor|Worker)\\s*$")]
    public string AssignedRole { get; init; }

    [StringLength(1000, MinimumLength = 1)]
    [RegularExpression("^(?=.*\\S)[^\\r\\n]+$")]
    public string? ReviewNotes { get; init; }
}

public sealed record RejectJoinRequestRequest
{
    public RejectJoinRequestRequest(string reason, string? reviewNotes = null)
    {
        Reason = reason;
        ReviewNotes = reviewNotes;
    }

    [Required]
    [StringLength(500, MinimumLength = 1)]
    [RegularExpression("^(?=.*\\S)[^\\r\\n]+$")]
    public string Reason { get; init; }

    [StringLength(1000, MinimumLength = 1)]
    [RegularExpression("^(?=.*\\S)[^\\r\\n]+$")]
    public string? ReviewNotes { get; init; }
}

public sealed record SubmitJoinRequestRequest
{
    public SubmitJoinRequestRequest(
        string companyCode,
        string fullName,
        string phoneNumber,
        string? email,
        string password,
        string requestedRole,
        string? requestMessage)
    {
        CompanyCode = companyCode;
        FullName = fullName;
        PhoneNumber = phoneNumber;
        Email = email;
        Password = password;
        RequestedRole = requestedRole;
        RequestMessage = requestMessage;
    }

    [Required]
    [StringLength(40, MinimumLength = 6)]
    [RegularExpression("^\\s*[A-Za-z0-9]{6,20}\\s*$")]
    public string CompanyCode { get; init; }

    [Required]
    [StringLength(200, MinimumLength = 1)]
    [RegularExpression("^(?=.*\\S)[^\\r\\n]+$")]
    public string FullName { get; init; }

    [Required]
    [StringLength(40)]
    [RegularExpression("^\\s*\\+[1-9][0-9]{7,14}\\s*$")]
    public string PhoneNumber { get; init; }

    [EmailAddress]
    [StringLength(254)]
    public string? Email { get; init; }

    [Required]
    [StringLength(128, MinimumLength = 12)]
    public string Password { get; init; }

    [Required]
    [StringLength(30)]
    [RegularExpression("^\\s*(Deputy|Accountant|Supervisor|Worker)\\s*$")]
    public string RequestedRole { get; init; }

    [StringLength(1000, MinimumLength = 1)]
    [RegularExpression("^(?=.*\\S)[^\\r\\n]+$")]
    public string? RequestMessage { get; init; }
}

public abstract record PagedAccessQuery
{
    [Range(1, int.MaxValue)]
    public int Page { get; init; } = 1;

    [Range(1, AccessConstants.MaximumPageSize)]
    public int PageSize { get; init; } = AccessConstants.DefaultPageSize;
}

public sealed record InvitationQuery : PagedAccessQuery
{
    [RegularExpression("^(Pending|Accepted|Expired|Cancelled)$")]
    public string? Status { get; init; }
}

public sealed record JoinRequestQuery : PagedAccessQuery
{
    [RegularExpression("^(Pending|Approved|Rejected|Cancelled)$")]
    public string? Status { get; init; }
}
