
using NativeSceneAutomation.Models;
using WorkflowCore.Interface;

namespace NativeSceneAutomation
{
    public class Worker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<Worker> _logger;

        public Worker(IServiceProvider serviceProvider, ILogger<Worker> logger)
        {
            _serviceProvider = serviceProvider; 
            _logger = logger;    
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

            await _serviceProvider.GetRequiredService<IWorkflowHost>().StartAsync(stoppingToken);
            await _serviceProvider.GetRequiredService<IHWService>().StartAsync();

            try
            {
                while (stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(1000, stoppingToken);
                }            
            }
            catch (TaskCanceledException)
            {}
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Worker start at: {time}", DateTimeOffset.Now);

            await _serviceProvider.GetRequiredService<IHWService>().InitializeAsync();
            await _serviceProvider.GetRequiredService<IHWService>().TurnOffPixelsAsync();

            await base.StartAsync(cancellationToken);
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Worker stopping at: {time}", DateTimeOffset.Now);

            await _serviceProvider.GetRequiredService<IWorkflowHost>().StopAsync(CancellationToken.None);
            await _serviceProvider.GetRequiredService<IHWService>().StopAsync();

            await base.StopAsync(stoppingToken);
        }
    }
}
