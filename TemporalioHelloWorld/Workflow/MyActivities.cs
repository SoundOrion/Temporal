using System;
using System.Threading.Tasks;
using Temporalio.Activities;

namespace SampleApp;

public class MyActivities : IMyActivities
{
    [Activity]
    public async Task<string> ProcessAAsync(string input)
    {
        await Task.Delay(200);
        return $"A({input})";
    }

    [Activity]
    public async Task<string> ProcessBAsync(string input)
    {
        await Task.Delay(200);
        return $"B({input})";
    }

    [Activity]
    public async Task<string> ProcessCAsync(string a, string b)
    {
        await Task.Delay(200);
        return $"C({a}+{b})";
    }

    [Activity]
    public async Task<string> UnstableAsync(string name)
    {
        await Task.Delay(100);
        if (Random.Shared.NextDouble() < 0.3) // 30%で失敗を模擬
            throw new Temporalio.Exceptions.ApplicationFailureException(
                "Transient failure", nonRetryable: false);
        return $"OK:{name}";
    }
}
