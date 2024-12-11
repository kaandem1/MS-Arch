using MCMS.Services;

public class RabbitMQBackgroundService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private Task _executingTask;
    private CancellationTokenSource _cts;

    public RabbitMQBackgroundService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _executingTask = ExecuteAsync(_cts.Token);
        return _executingTask.IsCompleted ? _executingTask : Task.CompletedTask;
    }

    private async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var rabbitMQConsumer = scope.ServiceProvider.GetRequiredService<RabbitMQConsumer>();
            try
            {
                await rabbitMQConsumer.StartListeningAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error starting RabbitMQ consumer: {ex.Message}");
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _cts.Cancel();
        return Task.CompletedTask;
    }
}
