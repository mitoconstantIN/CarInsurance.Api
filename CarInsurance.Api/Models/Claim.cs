namespace CarInsurance.Api.Models;

public sealed class Claim
{
    public long Id { get; set; }
    public long CarId { get; set; }  
    public DateOnly ClaimDate { get; set; }
    public string Description { get; set; } = default!;
    public decimal Amount { get; set; }

    public Car Car { get; set; } = default!;
}
