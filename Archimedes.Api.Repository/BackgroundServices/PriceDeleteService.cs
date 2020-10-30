using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Archimedes.Api.Repository.BackgroundServices
{
    public class PriceDeleteService: IHostedService
    {
        private readonly IUnitOfWork _unit;
        private readonly Logger<PriceDeleteService> _logger;

        public PriceDeleteService(IUnitOfWork unit, Logger<PriceDeleteService> logger)
        {
            _unit = unit;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var prices = await _unit.Price.GetPricesOlderThanOneHour(cancellationToken);
                    _unit.Price.RemovePrices(prices);
                    _unit.SaveChanges();
                    _logger.LogInformation($"Deleted {prices.Count} historic prices from Table ");

                    await Task.Delay(3600000, cancellationToken);
                }
            }
            catch (Exception e)
            {
                _logger.LogInformation($"Error deleting historic prices {e.Message} {e.StackTrace}");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}