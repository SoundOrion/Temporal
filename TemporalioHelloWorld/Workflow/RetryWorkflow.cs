using System;
using System.Threading.Tasks;
using Temporalio.Common;
using Temporalio.Workflows;

namespace SampleApp;

[Workflow]
public class RetryWorkflow
{
    private static readonly ActivityOptions RetryOpt = new()
    {
        StartToCloseTimeout = TimeSpan.FromSeconds(30),
        RetryPolicy = new RetryPolicy
        {
            InitialInterval = TimeSpan.FromSeconds(2),
            BackoffCoefficient = 2.0f,
            MaximumInterval = TimeSpan.FromSeconds(15),
            MaximumAttempts = 5   // 最大5回トライ
        }
    };

    [WorkflowRun]
    public async Task<string> RunAsync(string name)
    {
        var a = await Workflow.ExecuteActivityAsync(
            (MyActivities acts) => acts.UnstableAsync($"A-{name}"), RetryOpt);

        var b = await Workflow.ExecuteActivityAsync(
            (MyActivities acts) => acts.UnstableAsync($"B-{a}"), RetryOpt);

        var c = await Workflow.ExecuteActivityAsync(
            (MyActivities acts) => acts.UnstableAsync($"C-{b}"), RetryOpt);

        return c;
    }
}
