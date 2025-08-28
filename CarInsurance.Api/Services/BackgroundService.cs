using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using CarInsurance.Api.Data; // repo/context
using Microsoft.EntityFrameworkCore;

public class PolicyExpirationMonitor : BackgroundService
{
    private readonly ILogger<PolicyExpirationMonitor> _logger;
    private readonly IServiceProvider _serviceProvider;
    private static DateTime _lastRun = DateTime.MinValue;

    public PolicyExpirationMonitor(ILogger<PolicyExpirationMonitor> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CheckExpiredPolicies(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in PolicyExpirationMonitor");
            }

            await Task.Delay(TimeSpan.FromMinutes(30), stoppingToken);
        }
    }

    private async Task CheckExpiredPolicies(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var now = DateTime.UtcNow;
        var oneHourAgo = now.AddHours(-1);

        var policies = await db.Policies
            .Include(p => p.Car)
            .ThenInclude(c => c.Owner)
            .ToListAsync(cancellationToken);

        var recentlyExpired = policies
            .Select(p => new
            {
                Policy = p,
                ExpiredAt = DateTime.SpecifyKind(
                    p.EndDate.ToDateTime(new TimeOnly(23, 59, 59)),
                    DateTimeKind.Utc)
            })
            .Where(x => x.ExpiredAt <= now && x.ExpiredAt > oneHourAgo)
            .ToList();

        // 3) Log
        foreach (var x in recentlyExpired)
        {
            var mins = (int)(now - x.ExpiredAt).TotalMinutes;

            _logger.LogWarning(
                "Policy expired! PolicyId={PolicyId}, Provider={Provider}, Owner={Owner}, Car={Make} {Model} {Year}, VIN={VIN}, ExpiredAt={ExpiredAt:o}, Since={SinceMinutes} minutes",
                x.Policy.Id,
                x.Policy.Provider ?? "(unknown)",
                x.Policy.Car.Owner?.Name ?? $"OwnerId:{x.Policy.Car.OwnerId}",
                x.Policy.Car.Make ?? "(unknown)",
                x.Policy.Car.Model ?? "(unknown)",
                x.Policy.Car.YearOfManufacture,
                x.Policy.Car.Vin,
                x.ExpiredAt,
                mins
            );
        }
    }




}
