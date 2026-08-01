enum PendingResultKind {
  joinRequestSubmitted,
  identityVerificationRequired,
  managerApprovalRequired,
  generic,
}

final class PendingOnboardingResult {
  const PendingOnboardingResult(this.kind);

  final PendingResultKind kind;
}
