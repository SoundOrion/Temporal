using System;
using System.Threading.Tasks;
using Temporalio.Activities;

public class DataActivities
{
    [Activity] public async Task<string> ProcessAAsync(string input) { await Task.Delay(200); return $"A({input})"; }
    [Activity] public async Task<string> ProcessBAsync(string input) { await Task.Delay(200); return $"B({input})"; }
    [Activity] public async Task<string> ProcessCAsync(string a, string b) { await Task.Delay(200); return $"C({a}+{b})"; }
}
