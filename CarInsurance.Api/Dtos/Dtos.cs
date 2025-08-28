namespace CarInsurance.Api.Dtos;

public record CarDto(long Id, string Vin, string? Make, string? Model, int Year, long OwnerId, string OwnerName, string? OwnerEmail);
public record InsuranceValidityResponse(long CarId, string Date, bool Valid);

public sealed record CreateClaimRequest(DateOnly ClaimDate, string Description, decimal Amount);
public sealed record ClaimDto(long Id, DateOnly ClaimDate, string Description, decimal Amount);

public sealed record PolicyPeriodDto(DateOnly StartDate, DateOnly EndDate, string Provider);
public sealed record ClaimShortDto(long Id, DateOnly ClaimDate, string Description, decimal Amount);

public sealed record CarHistoryDto(
    long CarId,
    IReadOnlyList<PolicyPeriodDto> Policies,   // ordonate după StartDate ↑
    IReadOnlyList<ClaimShortDto> Claims       // ordonate după ClaimDate ↓ (noi → vechi)
);