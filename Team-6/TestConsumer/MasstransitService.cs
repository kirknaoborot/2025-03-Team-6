using MassTransit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace TestConsumer
{
    internal class MasstransitService : IHostedService
    {
        private IBusControl _busControl;
        private readonly ILogger<MasstransitService> _logger;

        public MasstransitService(IBusControl busControl, ILogger<MasstransitService> logger)
        {
            _busControl = busControl;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _busControl.StartAsync(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
                await _busControl.StopAsync(cancellationToken);
        }
    }
}
