using System.Threading;
using System.Threading.Tasks;
using SampleApp;
using Temporalio.Client;
using Temporalio.Worker;

var client = await TemporalClient.ConnectAsync(new("localhost:7233"));
using var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) => { e.Cancel = true; cts.Cancel(); };

var acts = new MyActivities();

using var worker = new TemporalWorker(
    client,
    new TemporalWorkerOptions("demo-task-queue")
        .AddAllActivities(acts) // IMyActivities 実装を登録
        .AddWorkflow<SequentialWorkflow>()
        .AddWorkflow<ParallelThenCWorkflow>()
        .AddWorkflow<RetryWorkflow>());

Console.WriteLine("Worker running. Press Ctrl+C to exit.");
await worker.ExecuteAsync(cts.Token);
