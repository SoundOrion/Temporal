using System.Threading;
using System.Threading.Tasks;
using Temporalio.Client;
using Temporalio.Worker;

public class TemporalWorkerHostedService : BackgroundService
{
    private readonly TemporalClient _client;
    private TemporalWorker? _worker;

    public TemporalWorkerHostedService(TemporalClient client)
    {
        _client = client;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _worker = new TemporalWorker(
            _client,
            new TemporalWorkerOptions("webapi-queue")
                .AddAllActivities(new DataActivities())
                .AddWorkflow<AbcWorkflow>());

        await _worker.ExecuteAsync(stoppingToken);
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _worker?.Dispose(); // v1.7は DisposeAsync が無い場合あり→DisposeでOK
        return base.StopAsync(cancellationToken);
    }
}
