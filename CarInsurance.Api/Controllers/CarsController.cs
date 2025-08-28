using CarInsurance.Api.Dtos;
using CarInsurance.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace CarInsurance.Api.Controllers;

[ApiController]
[Route("api")]
public class CarsController(CarService service) : ControllerBase
{
    private readonly CarService _service = service;

    [HttpGet("cars")]
    public async Task<ActionResult<List<CarDto>>> GetCars()
        => Ok(await _service.ListCarsAsync());

    [HttpGet("cars/{carId:long}/insurance-valid")]
    public async Task<ActionResult<InsuranceValidityResponse>> IsInsuranceValid(
        long carId,
        [FromQuery] DateOnly date,
        CancellationToken ct)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var min = today.AddYears(-100);
        var max = today.AddYears(+100);
        if (date < min || date > max)
            return BadRequest(new { error = $"Date out of range [{min:yyyy-MM-dd}, {max:yyyy-MM-dd}]." });

        try
        {
            var valid = await _service.IsInsuranceValidAsync(carId, date, ct);
            return Ok(new InsuranceValidityResponse(carId, date.ToString("yyyy-MM-dd"), valid));
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { error = "Car not found" });
        }
    }



    [HttpPost("cars/{carId:long}/claims")]
    public async Task<IActionResult> CreateClaim(
        long carId,
        [FromBody] CreateClaimRequest request,
        [FromServices] IClaimsService svc,
        CancellationToken ct)
    {
        try
        {
            var dto = await svc.CreateAsync(carId, request, ct);
            return Created($"/api/cars/{carId}/claims/{dto.Id}", dto);
        }
        catch (KeyNotFoundException) { return NotFound(new { error = "Car not found" }); }
        catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
    }


    [HttpGet("cars/{carId:long}/history")]
    public async Task<IActionResult> GetHistory(
        long carId,
        [FromServices] ICarHistoryService svc,
        CancellationToken ct)
    {
        try
        {
            var dto = await svc.GetAsync(carId, ct);
            return Ok(dto);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { error = "Car not found" });
        }
    }

}
