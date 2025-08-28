using CarInsurance.Api.Data;
using CarInsurance.Api.Dtos;
using Microsoft.EntityFrameworkCore;

namespace CarInsurance.Api.Services;

public class CarService(AppDbContext db)
{
    private readonly AppDbContext _db = db;

    public async Task<List<CarDto>> ListCarsAsync()
    {
        return await _db.Cars.Include(c => c.Owner)
            .Select(c => new CarDto(c.Id, c.Vin, c.Make, c.Model, c.YearOfManufacture,
                                    c.OwnerId, c.Owner.Name, c.Owner.Email))
            .ToListAsync();
    }

    public async Task<bool> IsInsuranceValidAsync(long carId, DateOnly date, CancellationToken ct = default)
    {
        var carExists = await _db.Cars.AnyAsync(c => c.Id == carId, ct);
        if (!carExists) throw new KeyNotFoundException("Car not found");

        var valid = await _db.Policies
            .Where(p => p.CarId == carId
                        && p.StartDate <= date
                        && p.EndDate   >= date)
            .AnyAsync(ct);

        return valid;
    }
}
