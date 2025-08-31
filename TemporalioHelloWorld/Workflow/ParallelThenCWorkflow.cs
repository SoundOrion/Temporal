using System;
using System.Threading.Tasks;
using Temporalio.Workflows;

namespace SampleApp;

[Workflow]
public class ParallelThenCWorkflow
{
    private static readonly ActivityOptions Opt =
        new() { StartToCloseTimeout = TimeSpan.FromMinutes(5) };

    [WorkflowRun]
    public async Task<string> RunAsync(string input)
    {
        // A と B を同時開始
        var taskA = Workflow.ExecuteActivityAsync(
            (MyActivities acts) => acts.ProcessAAsync(input), Opt);

        var taskB = Workflow.ExecuteActivityAsync(
            (MyActivities acts) => acts.ProcessBAsync(input), Opt);

        await Task.WhenAll(taskA, taskB);

        // 両方の結果を使って C
        var c = await Workflow.ExecuteActivityAsync(
            (MyActivities acts) => acts.ProcessCAsync(taskA.Result, taskB.Result), Opt);

        return c; // 例: C(A(x)+B(x))
    }
}
