
public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IServiceProvider _serviceProvider;

    public Worker(ILogger<Worker> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("�������� �������� ����������...");

            try
            {
                // ������ scope, ����� ������������ scoped-�������
                using var scope = _serviceProvider.CreateScope();
                var monitoringService = scope.ServiceProvider.GetRequiredService<OperatorMonitoringService>();
                await monitoringService.MonitorOperatorsAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "������ ��� ����������� ����������");
            }

            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }
    }
}