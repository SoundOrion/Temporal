using System.Threading.Tasks;

namespace SampleApp;

public interface IMyActivities
{
    Task<string> ProcessAAsync(string input);
    Task<string> ProcessBAsync(string input);
    Task<string> ProcessCAsync(string a, string b);

    // リトライ用：たまに失敗する処理
    Task<string> UnstableAsync(string name);
}
