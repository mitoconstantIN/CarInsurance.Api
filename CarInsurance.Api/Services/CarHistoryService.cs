using CarInsurance.Api.Data;
using CarInsurance.Api.Dtos;
using Microsoft.EntityFrameworkCore;

namespace CarInsurance.Api.Services;

public interface ICarHistoryService
{
    Task<CarHistoryDto> GetAsync(long carId, CancellationToken ct);
}

public sealed class CarHistoryService(AppDbContext db) : ICarHistoryService
{
    public async Task<CarHistoryDto> GetAsync(long carId, CancellationToken ct)
    {
        var carExists = await db.Cars.AnyAsync(c => c.Id == carId, ct);
        if (!carExists) throw new KeyNotFoundException();

        var policies = await db.Policies
            .Where(p => p.CarId == carId)
            .OrderBy(p => p.StartDate)
            .Select(p => new PolicyPeriodDto(p.StartDate, p.EndDate, p.Provider ?? "Unknown")) // ← fix
            .ToListAsync(ct);


        var claims = await db.Claims
            .Where(c => c.CarId == carId)
            .OrderByDescending(c => c.ClaimDate)
            .Select(c => new ClaimShortDto(c.Id, c.ClaimDate, c.Description, c.Amount))
            .ToListAsync(ct);

        return new CarHistoryDto(carId, policies, claims);
    }
}
