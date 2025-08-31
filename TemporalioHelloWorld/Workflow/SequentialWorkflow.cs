using System;
using System.Threading.Tasks;
using Temporalio.Workflows;

namespace SampleApp;

[Workflow]
public class SequentialWorkflow
{
    private static readonly ActivityOptions Opt =
        new() { StartToCloseTimeout = TimeSpan.FromMinutes(5) };

    [WorkflowRun]
    public async Task<string> RunAsync(string input)
    {
        var a = await Workflow.ExecuteActivityAsync(
            (MyActivities acts) => acts.ProcessAAsync(input), Opt);

        var b = await Workflow.ExecuteActivityAsync(
            (MyActivities acts) => acts.ProcessBAsync(a), Opt);

        var c = await Workflow.ExecuteActivityAsync(
            (MyActivities acts) => acts.ProcessCAsync(a, b), Opt);

        return c; // 例: C(A(x)+B(A(x)))
    }
}
