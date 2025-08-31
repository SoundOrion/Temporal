using System;
using System.Threading.Tasks;
using Temporalio.Workflows;

[Workflow]
public class AbcWorkflow
{
    private bool _approved;

    static readonly ActivityOptions Opt = new() { StartToCloseTimeout = TimeSpan.FromMinutes(5) };

    [WorkflowRun]
    public async Task<string> RunAsync(string input)
    {
        var a = await Workflow.ExecuteActivityAsync((DataActivities acts) => acts.ProcessAAsync(input), Opt);
        var b = await Workflow.ExecuteActivityAsync((DataActivities acts) => acts.ProcessBAsync(a), Opt);
        var c = await Workflow.ExecuteActivityAsync((DataActivities acts) => acts.ProcessCAsync(a, b), Opt);

        // 例：承認が必要なら待つ
        await Workflow.WaitConditionAsync(() => _approved);
        return c;
    }

    [WorkflowSignal] public Task Approve() { _approved = true; return Task.CompletedTask; }

    [WorkflowQuery] public object GetStatus() => new { approved = _approved };
}
