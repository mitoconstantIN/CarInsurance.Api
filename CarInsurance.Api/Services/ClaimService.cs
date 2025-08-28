using CarInsurance.Api.Data;
using CarInsurance.Api.Dtos;
using CarInsurance.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CarInsurance.Api.Services;

public interface IClaimsService
{
    Task<ClaimDto> CreateAsync(long carId, CreateClaimRequest req, CancellationToken ct);
}

public sealed class ClaimsService(AppDbContext db) : IClaimsService
{
    public async Task<ClaimDto> CreateAsync(long carId, CreateClaimRequest req, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(req.Description)) throw new ArgumentException("Description is required.");
        if (req.Amount <= 0) throw new ArgumentException("Amount must be > 0.");

        var carExists = await db.Cars.AnyAsync(c => c.Id == carId, ct);
        if (!carExists) throw new KeyNotFoundException("Car not found.");

        var entity = new Claim
        {
            CarId = carId,
            ClaimDate = req.ClaimDate,
            Description = req.Description.Trim(),
            Amount = req.Amount
        };
        db.Claims.Add(entity);
        await db.SaveChangesAsync(ct);

        return new ClaimDto(entity.Id, entity.ClaimDate, entity.Description, entity.Amount);
    }
}
