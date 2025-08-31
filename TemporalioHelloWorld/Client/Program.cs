using System;
using SampleApp;
using Temporalio.Client;

var client = await TemporalClient.ConnectAsync(new("localhost:7233"));
string NewId(string p) => $"{p}-{Guid.NewGuid()}";

// 1) 直列
var r1 = await client.ExecuteWorkflowAsync(
    (SequentialWorkflow wf) => wf.RunAsync("x"),
    new(id: NewId("seq"), taskQueue: "demo-task-queue"));
Console.WriteLine($"Sequential: {r1}");

// 2) 並列 → C
var r2 = await client.ExecuteWorkflowAsync(
    (ParallelThenCWorkflow wf) => wf.RunAsync("y"),
    new(id: NewId("par"), taskQueue: "demo-task-queue"));
Console.WriteLine($"Parallel-then-C: {r2}");

// 3) リトライ（最大試行超過で例外になる可能性あり）
try
{
    var r3 = await client.ExecuteWorkflowAsync(
        (RetryWorkflow wf) => wf.RunAsync("z"),
        new(id: NewId("retry"), taskQueue: "demo-task-queue"));
    Console.WriteLine($"Retry: {r3}");
}
catch (Exception ex)
{
    Console.WriteLine($"Retry workflow failed: {ex.Message}");
}
